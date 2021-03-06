using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ao.SavableConfig.Binder
{
    public class ProxyHelper
    {
        private static readonly Type INameTransferType = typeof(INameTransfer);
        private static readonly Type ObjectType = typeof(object);
        private static readonly Type StringType = typeof(string);
        private static readonly Type IConfigurationType = typeof(IConfiguration);
        private static readonly Type[] ProxyConstructTypes = new Type[] { INameTransferType, IConfigurationType };

        private static readonly PropertyInfo ConfigurationIndexProperty = IConfigurationType.GetProperties().First(x => x.GetIndexParameters().Length == 1);
        private static readonly MethodInfo NameTransferTransferMethod = INameTransferType.GetMethod(nameof(INameTransfer.Transfer));
        private static readonly MethodInfo ConfigurationBinderGetValueMethod = typeof(FastConfigurationBinder).GetMethod(nameof(FastConfigurationBinder.FastGetValue), new Type[] { typeof(IConfiguration), typeof(string) });
        private static readonly MethodAttributes PropertyMethodAttr = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
        private static readonly AssemblyBuilder DefaultDynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AoDynamicAssembly"), AssemblyBuilderAccess.Run);
        private static readonly ModuleBuilder DefaultDynamicModule = DefaultDynamicAssembly.DefineDynamicModule("AoDyModule");

        public static readonly ProxyHelper Default = new ProxyHelper(DefaultDynamicAssembly, DefaultDynamicModule);
        private readonly ConstructCompileManager constructCompileManager;

        public AssemblyBuilder DynamicAssembly { get; }
        public ModuleBuilder DynamicModule { get; }

        private readonly Dictionary<Type, Type> proxMap;
        private readonly Dictionary<Type, ConstructorInfo> constructorMap;

        public IReadOnlyDictionary<Type, Type> ProxMap => proxMap;

        public ProxyHelper(AssemblyBuilder dynamicAssembly, ModuleBuilder dynamicModule)
        {
            DynamicAssembly = dynamicAssembly ?? throw new ArgumentNullException(nameof(dynamicAssembly));
            DynamicModule = dynamicModule ?? throw new ArgumentNullException(nameof(dynamicModule));
            proxMap = new Dictionary<Type, Type>();
            constructCompileManager = new ConstructCompileManager();
            constructorMap = new Dictionary<Type, ConstructorInfo>();
        }

        public bool HasTypeProxy(Type type)
        {

            Debug.Assert(proxMap != null);
            return proxMap.ContainsKey(type);
        }
        public Type GetProxyType(Type type)
        {
            if (HasTypeProxy(type))
            {
                return proxMap[type];
            }
            return null;
        }
        public object CreateProxy(Type type, IConfiguration configuration, INameTransfer nameTransfer)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (nameTransfer is null)
            {
                throw new ArgumentNullException(nameof(nameTransfer));
            }

            if (proxMap.TryGetValue(type, out var proxType))
            {
                return CreateInstance(proxType, configuration, nameTransfer);
            }
            return null;
        }
        public bool BuildProx(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!HasTypeProxy(type))
            {
                Build(type);
                return true;
            }
            return false;
        }
        protected virtual object CreateInstance(Type type, IConfiguration configuration, INameTransfer nameTransfer)
        {
            var c = constructCompileManager.GetCompiled(constructorMap[type]);
            return c.Creator(nameTransfer, configuration);
        }
        private Type Build(Type type)
        {
            Debug.Assert(type != null);
            var @class = DynamicModule.DefineType("Prox" + type.Name + type.GetHashCode(), TypeAttributes.NotPublic | TypeAttributes.Sealed);
            @class.SetParent(type);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var transferField = @class.DefineField("transfer", INameTransferType, FieldAttributes.Private | FieldAttributes.InitOnly);
            var configurationField = @class.DefineField("configuration", IConfigurationType, FieldAttributes.Private | FieldAttributes.InitOnly);
            var constract = @class.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ProxyConstructTypes);
            var objectConst = ObjectType.GetConstructor(Type.EmptyTypes);
            var constractIl = constract.GetILGenerator();
            constractIl.Emit(OpCodes.Ldarg_0);
            constractIl.Emit(OpCodes.Call, objectConst);
            constractIl.Emit(OpCodes.Ldarg_0);
            constractIl.Emit(OpCodes.Ldarg_1);
            constractIl.Emit(OpCodes.Stfld, transferField);
            constractIl.Emit(OpCodes.Ldarg_0);
            constractIl.Emit(OpCodes.Ldarg_2);
            constractIl.Emit(OpCodes.Stfld, configurationField);
            constractIl.Emit(OpCodes.Ret);

            var len = properties.Length;

            for (var i = 0; i < len; i++)
            {
                var item = properties[i];
                if (!TypeHelper.IsBaseType(item.PropertyType))
                {
                    continue;
                }
                var implExtGetMethod = ConfigurationBinderGetValueMethod.MakeGenericMethod(item.PropertyType);
                if (item.GetMethod.IsVirtual && item.GetMethod.IsPublic)
                {
                    var get = @class.DefineMethod("get_" + item.Name, PropertyMethodAttr, item.PropertyType, Type.EmptyTypes);
                    var getIl = get.GetILGenerator();
                    getIl.DeclareLocal(StringType);
                    getIl.DeclareLocal(IConfigurationType);

                    getIl.Emit(OpCodes.Ldarg_0);
                    getIl.Emit(OpCodes.Ldfld, transferField);
                    getIl.Emit(OpCodes.Ldarg_0);
                    getIl.Emit(OpCodes.Ldstr, item.Name);
                    getIl.Emit(OpCodes.Callvirt, NameTransferTransferMethod);
                    getIl.Emit(OpCodes.Stloc_0);

                    getIl.Emit(OpCodes.Ldarg_0);
                    getIl.Emit(OpCodes.Ldfld, configurationField);
                    getIl.Emit(OpCodes.Ldloc_0);
                    getIl.Emit(OpCodes.Call, implExtGetMethod);
                    getIl.Emit(OpCodes.Ret);
                }
                if (item.SetMethod.IsVirtual && item.SetMethod.IsPublic)
                {
                    var set = @class.DefineMethod("set_" + item.Name, PropertyMethodAttr, null, new Type[] { item.PropertyType });
                    var setIl = set.GetILGenerator();
                    setIl.DeclareLocal(StringType);
                    setIl.DeclareLocal(StringType);

                    setIl.Emit(OpCodes.Ldarg_0);
                    setIl.Emit(OpCodes.Ldfld, transferField);
                    setIl.Emit(OpCodes.Ldarg_0);
                    setIl.Emit(OpCodes.Ldstr, item.Name);
                    setIl.Emit(OpCodes.Callvirt, NameTransferTransferMethod);
                    setIl.Emit(OpCodes.Stloc_0);

                    var isStringType = item.PropertyType == StringType;
                    if (isStringType)
                    {
                        setIl.Emit(OpCodes.Ldarg_1);
                    }
                    else
                    {
                        var toStr = item.PropertyType.GetMethod(nameof(object.ToString), Type.EmptyTypes);
                        setIl.Emit(OpCodes.Ldarga_S, 1);
                        setIl.Emit(OpCodes.Callvirt, toStr);
                    }
                    setIl.Emit(OpCodes.Stloc_1);


                    setIl.Emit(OpCodes.Ldarg_0);
                    setIl.Emit(OpCodes.Ldfld, configurationField);
                    setIl.Emit(OpCodes.Ldloc_0);
                    setIl.Emit(OpCodes.Ldloc_1);
                    setIl.Emit(OpCodes.Callvirt, ConfigurationIndexProperty.SetMethod);

                    setIl.Emit(OpCodes.Ldarg_0);
                    setIl.Emit(OpCodes.Ldarg_1);
                    setIl.Emit(OpCodes.Call, item.SetMethod);
                    setIl.Emit(OpCodes.Ret);
                }
            }
            var proxType = @class.CreateTypeInfo();
            var constInfo = proxType.GetConstructor(ProxyConstructTypes);
            constructCompileManager.EnsureGetCompiled(constInfo);
            constructorMap.Add(proxType, constInfo);
            proxMap.Add(type, proxType);
            return proxType;
        }
    }
}
