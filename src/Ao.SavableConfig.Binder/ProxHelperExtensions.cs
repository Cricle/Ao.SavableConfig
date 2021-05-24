using Ao.SavableConfig.Binder.Annotations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public static class ProxHelperExtensions
    {
        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper, bool autoAnalysis)
        {
            if (proxyHelper is null)
            {
                throw new ArgumentNullException(nameof(proxyHelper));
            }

            var type = new[] { typeof(T) };
            var objType = typeof(object);
            var map = new Dictionary<Type, INameTransfer>();
            map.Add(type[0], IdentityMapNameTransfer.FromTypeAttributes(type[0]));
            if (autoAnalysis)
            {
                while (type.Length != 0)
                {
                    var includeTypes = type.SelectMany(x =>
                        x.GetProperties()
                        .Where(y => y.CanWrite && y.CanRead && y.PropertyType.IsClass && x.GetCustomAttribute<ConfigStepInAttribute>()!=null||y.GetCustomAttribute<ConfigStepInAttribute>() != null&&!map.ContainsKey(y.PropertyType)))
                            .ToList();
                    var notInclues = new List<PropertyInfo>();
                    foreach (var item in includeTypes)
                    {
                        if (map.ContainsKey(item.PropertyType))
                        {
                            notInclues.Add(item);
                            continue;
                        }
                        var nameMap = IdentityMapNameTransfer.FromTypeAttributes(item.PropertyType);
                        map.Add(item.PropertyType, nameMap);
                    }
                    type = includeTypes.Except(notInclues).Select(x => x.DeclaringType).Distinct().ToArray();
                }
            }
            return CreateComplexProxy<T>(proxyHelper, map);
        }
        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper,IReadOnlyDictionary<Type,INameTransfer> nameTransferPicker)
        {
            if (proxyHelper is null)
            {
                throw new ArgumentNullException(nameof(proxyHelper));
            }

            return new ProxyCreator(proxyHelper, typeof(T), nameTransferPicker);
        }
        public static T EnsureCreateProxWithAttribute<T>(this ProxyHelper proxHelper, IConfiguration configuration)
            where T:class
        {
            if (proxHelper is null)
            {
                throw new ArgumentNullException(nameof(proxHelper));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var nameTransfer = IdentityMapNameTransfer.FromTypeAttributes(typeof(T));
            return EnsureCreateProx<T>(proxHelper, configuration, nameTransfer);
        }
        public static T EnsureCreateProx<T>(this ProxyHelper proxHelper, IConfiguration configuration, INameTransfer nameTransfer)
            where T:class
        {
            if (proxHelper is null)
            {
                throw new ArgumentNullException(nameof(proxHelper));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (nameTransfer is null)
            {
                throw new ArgumentNullException(nameof(nameTransfer));
            }

            var type = typeof(T);
            if (!proxHelper.HasTypeProxy(type))
            {
                proxHelper.BuildProx(type);
            }
            return (T)proxHelper.CreateProxy(typeof(T),configuration,nameTransfer);
        }
    }
}
