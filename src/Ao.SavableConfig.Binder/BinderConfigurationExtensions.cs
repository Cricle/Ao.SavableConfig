using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Ao.SavableConfig.Binder.Annotations;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public static class BinderConfigurationExtensions
    {
        public static T AutoCreateProxy<T>(this IConfiguration configuration, string section, INameTransfer nameTransfer = null)
                  where T : class
        {
            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentException($"“{nameof(section)}”不能为 Null 或空。", nameof(section));
            }

            return AutoCreateProxy<T>(configuration.GetSection(section), nameTransfer);
        }
        public static T AutoCreateProxy<T>(this IConfiguration configuration, INameTransfer nameTransfer = null)
            where T : class
        {
            var type = typeof(T);
            var forceStepIn = type.GetCustomAttribute<ConfigStepInAttribute>() != null;
            if (forceStepIn ||
                type.GetProperties().Any(x => x.CanWrite && x.PropertyType.IsClass && x.GetCustomAttributes<ConfigStepInAttribute>() != null))
            {
                return CreateComplexProxy<T>(configuration);
            }
            return CreateProxy<T>(configuration, nameTransfer);
        }
        public static T CreateComplexProxy<T>(this IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return ComplexProxyHelper.Default.Build<T>(configuration);
        }
        public static T CreateProxy<T>(this IConfiguration configuration, INameTransfer nameTransfer = null)
            where T : class
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (nameTransfer is null)
            {
                return ProxyHelper.Default.EnsureCreateProxWithAttribute<T>(configuration);
            }
            return ProxyHelper.Default.EnsureCreateProx<T>(configuration, nameTransfer);
        }
    }
}
