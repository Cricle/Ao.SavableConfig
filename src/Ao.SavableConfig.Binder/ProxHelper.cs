using Ao.SavableConfig.Binder.Annotations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ao.SavableConfig.Binder
{
    public class ProxyHelper
    {
        private static readonly Type INameTransferType = typeof(INameTransfer);
        private static readonly Type ObjectType = typeof(object);
        private static readonly Type StringType = typeof(string);
        private static readonly Type IConfigurationType = typeof(IConfiguration);
        private static readonly Type NullableType = typeof(Nullable<>);

        private static readonly PropertyInfo ConfigurationIndexProperty = IConfigurationType.GetProperties().Where(x => x.GetIndexParameters().Length == 1).First();
        private static readonly MethodInfo NameTransferTransferMethod = INameTransferType.GetMethod(nameof(INameTransfer.Transfer));
        private static readonly MethodInfo ConfigurationBinderGetValueMethod = typeof(ConfigurationBinder).GetMethod(nameof(ConfigurationBinder.GetValue), new Type[] { typeof(IConfiguration), typeof(string) });
        private static readonly MethodAttributes PropertyMethodAttr = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
        private static readonly MethodInfo ToStringMethod = ObjectType.GetMethod(nameof(object.ToString), Type.EmptyTypes);
        private static readonly AssemblyBuilder DefaultDynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AoDynamicAssembly"), AssemblyBuilderAccess.Run);
        private static readonly ModuleBuilder DefaultDynamicModule = DefaultDynamicAssembly.DefineDynamicModule("AoDyModule");

        public static readonly ProxyHelper Default = new ProxyHelper(DefaultDynamicAssembly, DefaultDynamicModule);

        public AssemblyBuilder DynamicAssembly { get; }
        public ModuleBuilder DynamicModule { get; }

        private readonly Dictionary<Type, Type> proxMap;

        public IReadOnlyDictionary<Type, Type> ProxMap => proxMap;

        public ProxyHelper(AssemblyBuilder dynamicAssembly, ModuleBuilder dynamicModule)
        {
            DynamicAssembly = dynamicAssembly ?? throw new ArgumentNullException(nameof(dynamicAssembly));
            DynamicModule = dynamicModule ?? throw new ArgumentNullException(nameof(dynamicModule));
            proxMap = new Dictionary<Type, Type>();
        }

        public bool HasTypeProxy(Type type)
        {
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
            if (proxMap.TryGetValue(type, out var proxType))
            {
                return CreateInstance(proxType, configuration, nameTransfer);
            }
            return null;
        }
        public bool BuildProx(Type type)
        {
            if (!HasTypeProxy(type))
            {
                Build(type);
                return true;
            }
            return false;
        }
        protected virtual object CreateInstance(Type type, IConfiguration configuration, INameTransfer nameTransfer)
        {
            return Activator.CreateInstance(type, nameTransfer, configuration);
        }
        private Type Build(Type type)
        {
            var @class = DynamicModule.DefineType("Prox" + type.Name + type.GetHashCode(), TypeAttributes.NotPublic | TypeAttributes.Sealed);
            @class.SetParent(type);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToArray();
            var transferField = @class.DefineField("transfer", INameTransferType, FieldAttributes.Private | FieldAttributes.InitOnly);
            var configurationField = @class.DefineField("configuration", IConfigurationType, FieldAttributes.Private | FieldAttributes.InitOnly);
            var constract = @class.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(INameTransfer), typeof(IConfiguration) });
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

            foreach (var item in properties)
            {
                if (!(item.PropertyType.IsGenericType &&
                    item.PropertyType.GetGenericTypeDefinition() == NullableType &&
                    CheckType(item.PropertyType.GenericTypeArguments[0]) ||
                    CheckType(item.PropertyType)))
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
                    if (!isStringType)
                    {
                        var toStr = item.PropertyType.GetMethod(nameof(object.ToString), Type.EmptyTypes);
                        setIl.Emit(OpCodes.Ldarga_S, 1);
                        setIl.Emit(OpCodes.Callvirt, toStr);
                    }
                    else
                    {
                        setIl.Emit(OpCodes.Ldarg_1);
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
            proxMap.Add(type, proxType);
            return proxType;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool CheckType(Type t)
        {
            return t.IsPrimitive || t == StringType;
        }
    }
}
