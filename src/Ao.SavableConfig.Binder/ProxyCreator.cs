using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Binder.Visitors;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class ProxyCreator
    {
        public ProxyCreator(ProxyHelper proxyHelper,
            Type type,
            INameTransfer nameTransfer,
            INamedCreator namedCreator,
            IPropertyVisitor propertyVisitor)
        {
            PropertyVisitor = propertyVisitor ?? throw new ArgumentNullException(nameof(propertyVisitor));
            NameTransfer = nameTransfer ?? throw new ArgumentNullException(nameof(nameTransfer));
            NamedCreator = namedCreator ?? throw new ArgumentNullException(nameof(namedCreator));
            ProxyHelper = proxyHelper ?? throw new ArgumentNullException(nameof(proxyHelper));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            propertyInfos = new Dictionary<PropertyInfo, IPropertyProxyInfo>();
            IsForceStepIn = type.GetCustomAttribute<ConfigStepInAttribute>() != null;
            ConfigPath = Type.GetCustomAttribute<ConfigPathAttribute>();
            ProxyHelper.BuildProx(Type);
            Analysis();
        }
        private readonly Dictionary<PropertyInfo, IPropertyProxyInfo> propertyInfos;

        public string CombineName { get; }

        public IPropertyVisitor PropertyVisitor { get; }

        public INameTransfer NameTransfer { get; }

        public bool IsForceStepIn { get; }

        public ConfigPathAttribute ConfigPath { get; }

        public bool HasConfigPath => ConfigPath != null;

        public INamedCreator NamedCreator { get; }

        public ProxyHelper ProxyHelper { get; }

        public IReadOnlyDictionary<PropertyInfo, IPropertyProxyInfo> PropertyInfos => propertyInfos;

        public Type Type { get; }

        private bool CanStepIn(PropertyInfo info)
        {
            return info.PropertyType.IsClass && info.CanWrite && info.CanRead && info.GetCustomAttribute<ConfigStepInAttribute>() != null;
        }
        private IEnumerable<PropertyInfo> AnalysisCore(Type type)
        {
            IEnumerable<PropertyInfo> props = type.GetProperties();
            if (!IsForceStepIn)
            {
                props = props.Where(x => CanStepIn(x));
            }
            var map = NamedCreator.Create(type, IsForceStepIn);
            foreach (var item in props)
            {
                ProxyHelper.BuildProx(item.PropertyType);
                var config = item.GetCustomAttribute<ConfigPathAttribute>();
                var name = item.Name;
                if (config != null && !string.IsNullOrEmpty(config.Name))
                {
                    name = config.Name;
                }
                var info = new PropertyProxyInfo();
                map.TryGetValue(item, out var transfer);
                info.NameTransfer = transfer ?? NullNameTransfer.Instance;
                if (!TypeHelper.IsBaseType(item.PropertyType) && (IsForceStepIn || CanStepIn(item)))
                {
                    info.ProxyCreator = new ProxyCreator(ProxyHelper,
                        item.PropertyType,
                        info.NameTransfer,
                        NamedCreator,
                        PropertyVisitor);
                }
                info.Key = ConfigPath?.Name;
                info.ParentProxyCreator = this;
                info.PropertyInfo = item;
                info.Identity = new PropertyIdentity(Type, item.Name);
                propertyInfos.Add(item, info);
            }
            return props;
        }
        private void Analysis()
        {
            AnalysisCore(Type);
        }
        private object BuildCore(Type type, IConfiguration configuration)
        {
            var inst = ProxyHelper.CreateProxy(type, configuration, NameTransfer);
            foreach (var item in propertyInfos.Values)
            {
                var selectConfig = configuration;
                if (!string.IsNullOrEmpty(item.Key))
                {
                    selectConfig = selectConfig.GetSection(item.Key);
                }
                if (item.ProxyCreator != null)
                {
                    var val = item.ProxyCreator.Build(selectConfig);
                    PropertyVisitor.SetValue(inst, val, item.PropertyInfo);
                }
            }
            return inst;
        }
        public virtual object Build(IConfiguration configuration)
        {
            return BuildCore(Type, configuration);
        }
    }
}
