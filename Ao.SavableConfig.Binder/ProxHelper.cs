using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Ao.SavableConfig.Binder
{
    public class ProxyHelper
    {
        private static readonly AssemblyBuilder DefaultDynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AoDynamicAssembly"), AssemblyBuilderAccess.RunAndCollect);
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
            var @class = DynamicModule.DefineType("Prox" + type.Name, TypeAttributes.Public | TypeAttributes.Sealed);
            @class.SetParent(type);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToArray();
            var transferMethod = typeof(INameTransfer).GetMethod(nameof(INameTransfer.Transfer));
            var transferField = @class.DefineField("transfer", typeof(INameTransfer), FieldAttributes.Private | FieldAttributes.InitOnly);
            var configurationField = @class.DefineField("configuration", typeof(IConfiguration), FieldAttributes.Private | FieldAttributes.InitOnly);
            var constract = @class.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(INameTransfer), typeof(IConfiguration) });
            var objectConst = typeof(object).GetConstructor(Type.EmptyTypes);
            var constractIl = constract.GetILGenerator();
            var constractThrowLabel = constractIl.DefineLabel();
            constractIl.Emit(OpCodes.Ldarg_0);
            constractIl.Emit(OpCodes.Call, objectConst);
            constractIl.Emit(OpCodes.Ldarg_0);
            constractIl.Emit(OpCodes.Ldarg_1);
            constractIl.Emit(OpCodes.Stfld, transferField);
            constractIl.Emit(OpCodes.Ldarg_0);
            constractIl.Emit(OpCodes.Ldarg_2);
            constractIl.Emit(OpCodes.Stfld, configurationField);
            constractIl.Emit(OpCodes.Ret);
            var configurationIndexProperty = typeof(IConfiguration)
                .GetProperties()
                .Where(x => x.GetIndexParameters().Length == 1)
                .First();

            foreach (var item in properties)
            {
                if (item.GetMethod.IsVirtual && item.GetMethod.IsPublic)
                {
                    var attr = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
                    var get = @class.DefineMethod("get_" + item.Name, attr, item.PropertyType, Type.EmptyTypes);
                    var getIl = get.GetILGenerator();
                    var loc = getIl.DeclareLocal(item.PropertyType);
                    getIl.Emit(OpCodes.Ldarg_0);
                    getIl.Emit(OpCodes.Ldfld, transferField);
                    getIl.Emit(OpCodes.Ldarg_0);
                    getIl.Emit(OpCodes.Ldstr, item.Name);
                    getIl.Emit(OpCodes.Callvirt, transferMethod);
                    getIl.Emit(OpCodes.Stloc_0);
                    getIl.Emit(OpCodes.Ldarg_0);
                    getIl.Emit(OpCodes.Ldfld, configurationField);
                    getIl.Emit(OpCodes.Ldloc_0);
                    getIl.Emit(OpCodes.Callvirt, configurationIndexProperty.GetMethod);
                    getIl.Emit(OpCodes.Ret);
                }
                if (item.SetMethod.IsVirtual && item.SetMethod.IsPublic)
                {
                    var attr = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
                    var set = @class.DefineMethod("set_" + item.Name, attr, typeof(void), new Type[] { item.PropertyType });
                    var setIl = set.GetILGenerator();
                    var loc = setIl.DeclareLocal(item.PropertyType);
                    setIl.Emit(OpCodes.Ldarg_0);
                    setIl.Emit(OpCodes.Ldfld, transferField);
                    setIl.Emit(OpCodes.Ldarg_0);
                    setIl.Emit(OpCodes.Ldstr, item.Name);
                    setIl.Emit(OpCodes.Callvirt, transferMethod);
                    setIl.Emit(OpCodes.Stloc_0);

                    setIl.Emit(OpCodes.Ldarg_0);
                    setIl.Emit(OpCodes.Ldfld, configurationField);
                    setIl.Emit(OpCodes.Ldloc_0);
                    setIl.Emit(OpCodes.Ldarg_1);
                    setIl.Emit(OpCodes.Callvirt, configurationIndexProperty.SetMethod);

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

    }
}
