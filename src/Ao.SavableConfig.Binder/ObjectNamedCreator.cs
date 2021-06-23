using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Binder.Visitors;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class ObjectNamedCreator
    {
        public static ObjectNamedCreator Create(Type type)
        {
            var transfer = IdentityMapNameTransfer.FromTypeAttributes(type);
            return new ObjectNamedCreator(type,
                transfer,
                IdentityNamedCreator.Instance,
                CompilePropertyVisitor.Instance);
        }
        public ObjectNamedCreator(Type type,
            INameTransfer nameTransfer,
            INamedCreator namedCreator,
            IPropertyVisitor propertyVisitor)
        {
            PropertyVisitor = propertyVisitor ?? throw new ArgumentNullException(nameof(propertyVisitor));
            NameTransfer = nameTransfer ?? throw new ArgumentNullException(nameof(nameTransfer));
            NamedCreator = namedCreator ?? throw new ArgumentNullException(nameof(namedCreator));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            propertyInfos = new Dictionary<PropertyInfo, IPropertyProxyInfo>();
            IsForceStepIn = type.GetCustomAttribute<ConfigStepInAttribute>() != null;
            ConfigPath = Type.GetCustomAttribute<ConfigPathAttribute>();
        }
        private readonly Dictionary<PropertyInfo, IPropertyProxyInfo> propertyInfos;

        public IPropertyVisitor PropertyVisitor { get; }

        public INameTransfer NameTransfer { get; }

        public bool IsForceStepIn { get; }

        public ConfigPathAttribute ConfigPath { get; }

        public bool HasConfigPath => ConfigPath != null;

        public INamedCreator NamedCreator { get; }

        public IReadOnlyDictionary<PropertyInfo, IPropertyProxyInfo> PropertyInfos => propertyInfos;

        public Type Type { get; }

        private bool CanStepIn(PropertyInfo info)
        {
            return info.PropertyType.IsClass && info.CanWrite && info.CanRead;
        }
        protected virtual void BuildType(Type type)
        {

        }
        protected virtual ObjectNamedCreator CreateCreator(PropertyInfo info, INameTransfer transfer)
        {
            return new ObjectNamedCreator(info.PropertyType,
                        transfer,
                        NamedCreator,
                        PropertyVisitor);
        }
        private PropertyInfo[] AnalysisCore(Type type)
        {
            var props = type.GetProperties();
            var map = NamedCreator.Create(type, IsForceStepIn);
            foreach (var item in props)
            {
                var config = item.GetCustomAttribute<ConfigPathAttribute>();
                var info = new PropertyProxyInfo();
                if (map.TryGetValue(item, out var transfer))
                {
                    info.NameTransfer = transfer;
                }
                else
                {
                    info.NameTransfer = NullNameTransfer.Instance;
                }
                if (!TypeHelper.IsBaseType(item.PropertyType) && (IsForceStepIn || CanStepIn(item)))
                {
                    BuildType(item.PropertyType);
                    info.ProxyCreator = CreateCreator(item, info.NameTransfer);
                    info.ProxyCreator.Analysis();
                }
                var name = item.Name;
                if (!string.IsNullOrEmpty(config?.Name))
                {
                    name = config.Name;
                }
                info.Key = ConfigPath?.Name ?? name;
                info.ParentProxyCreator = this;
                info.PropertyInfo = item;
                info.Identity = new PropertyIdentity(Type, item.Name);
                propertyInfos.Add(item, info);
            }
            return props;
        }
        public void Analysis()
        {
            propertyInfos.Clear();
            AnalysisCore(Type);
        }
        public object Build(object inst, IConfiguration configuration)
        {
            foreach (var item in propertyInfos.Values)
            {
                var selectConfig = configuration.GetSection(item.Key);
                var value = selectConfig.Value;

                if (item.ProxyCreator is null)
                {
                    if (!string.IsNullOrEmpty(value) &&
                        TypeHelper.TryChangeType(value, item.PropertyInfo.PropertyType, out _, out var res))
                    {
                        PropertyVisitor.SetValue(inst, res, item.PropertyInfo);
                    }
                }
                else
                {
                    var propVal = PropertyVisitor.GetValue(inst, item.PropertyInfo);
                    if (propVal is null)
                    {
                        var val = item.ProxyCreator.Build(selectConfig);
                        PropertyVisitor.SetValue(inst, val, item.PropertyInfo);
                    }
                    else
                    {
                        item.ProxyCreator.Build(propVal, selectConfig);
                    }

                }
            }
            return inst;
        }
        private object BuildCore(Type type, IConfiguration configuration)
        {
            var inst = CreateInstance(type, configuration);
            return Build(inst, configuration);
        }
        protected virtual object CreateInstance(Type type, IConfiguration configuration)
        {
            return Activator.CreateInstance(type);
        }
        public object Build(IConfiguration configuration)
        {
            return BuildCore(Type, configuration);
        }
    }
}
