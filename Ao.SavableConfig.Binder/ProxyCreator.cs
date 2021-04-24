using Ao.SavableConfig.Binder.Annotations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class ProxyCreator
    {
        public ProxyCreator(ProxyHelper proxyHelper, Type type, IReadOnlyDictionary<Type, INameTransfer> nameTransferPicker = null)
        {
            ProxyHelper = proxyHelper ?? throw new ArgumentNullException(nameof(proxyHelper));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            popertyProxyType = new Dictionary<PropertyInfo, string>();
            if (nameTransferPicker is null)
            {
                NameTransferPicker = new Dictionary<Type, INameTransfer>();
            }
            else
            {
                NameTransferPicker = nameTransferPicker;
            }
            Analysis();
        }

        private readonly Dictionary<PropertyInfo,string> popertyProxyType;

        public ProxyHelper ProxyHelper { get; }

        public IReadOnlyDictionary<Type, INameTransfer> NameTransferPicker { get; }

        public IReadOnlyDictionary<PropertyInfo,string> PopertyProxyType => popertyProxyType;

        public Type Type { get; }

        private void Analysis()
        {
            ProxyHelper.BuildProx(Type);
            var props = Type.GetProperties()
                .Where(x => x.PropertyType.IsClass && x.CanWrite && x.CanRead && x.GetCustomAttribute<ConfigStepInAttribute>() != null)
                .ToArray();
            foreach (var item in props)
            {
                var config = item.GetCustomAttribute<ConfigPathAttribute>();
                ProxyHelper.BuildProx(item.PropertyType);
                popertyProxyType.Add(item, config?.Name ?? item.Name);
            }
        }
        public virtual object Build(IConfiguration configuration)
        {
            if (!NameTransferPicker.TryGetValue(Type,out var nameTransfer))
            {
                nameTransfer = NullNameTransfer.Instance;
            }
            var inst = ProxyHelper.CreateProxy(Type, configuration, nameTransfer);
            foreach (var item in popertyProxyType)
            {
                if (!NameTransferPicker.TryGetValue(item.Key.PropertyType, out nameTransfer))
                {
                    nameTransfer = NullNameTransfer.Instance;
                }
                var selectConfig = configuration;
                if (!string.IsNullOrEmpty(item.Value))
                {
                    selectConfig = selectConfig.GetSection(item.Value);
                }
                var val = ProxyHelper.CreateProxy(item.Key.PropertyType, selectConfig, nameTransfer);
                SetValue(inst, val, item.Key);
            }
            return inst;
        }
        protected virtual void SetValue(object inst, object value, PropertyInfo propertyInfo)
        {
            propertyInfo.SetValue(inst, value);
        }
    }
}
