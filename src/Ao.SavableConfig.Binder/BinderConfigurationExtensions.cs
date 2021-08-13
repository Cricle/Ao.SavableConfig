using Ao.SavableConfig.Binder.Annotations;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public static class BinderConfigurationExtensions
    {
        public static T AutoCreateProxy<T>(this IConfiguration configuration, string section, INameTransfer nameTransfer)
            where T : class
        {
            return (T)AutoCreateProxy(configuration, typeof(T), section, nameTransfer);
        }
        public static object AutoCreateProxy(this IConfiguration configuration, Type type, string section, INameTransfer nameTransfer)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentException($"“{nameof(section)}”不能为 Null 或空。", nameof(section));
            }

            return AutoCreateProxy(configuration.GetSection(section), type, nameTransfer);
        }
        public static object AutoCreateProxy(this IConfiguration configuration, Type type, string section)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentException($"“{nameof(section)}”不能为 Null 或空。", nameof(section));
            }

            return AutoCreateProxy(configuration.GetSection(section), type, (INameTransfer)null);
        }
        public static T AutoCreateProxy<T>(this IConfiguration configuration, string section)
        {
            return (T)AutoCreateProxy(configuration, typeof(T), section);
        }
        public static object AutoCreateProxy(this IConfiguration configuration, Type type)
        {
            return AutoCreateProxy(configuration, type, (INameTransfer)null);
        }
        public static T AutoCreateProxy<T>(this IConfiguration configuration)
        {
            return (T)AutoCreateProxy(configuration, typeof(T));
        }
        public static object AutoCreateProxy(this IConfiguration configuration, Type type, INameTransfer nameTransfer)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var forceStepIn = type.GetCustomAttribute<ConfigStepInAttribute>() != null;
            if (forceStepIn ||
                type.GetProperties().Any(x => x.CanWrite && x.PropertyType.IsClass && x.GetCustomAttributes<ConfigStepInAttribute>() != null))
            {
                return CreateComplexProxy(configuration, type);
            }
            return CreateProxy(configuration, type, nameTransfer);
        }
        public static T AutoCreateProxy<T>(this IConfiguration configuration, INameTransfer nameTransfer)
        {
            return (T)AutoCreateProxy(configuration, typeof(T), nameTransfer);
        }
        public static object CreateComplexProxy(this IConfiguration configuration, Type type)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return ComplexProxyHelper.Default.Build(configuration, type);
        }
        public static T CreateComplexProxy<T>(this IConfiguration configuration)
        {
            return (T)CreateComplexProxy(configuration, typeof(T));
        }
        public static object CreateProxy(this IConfiguration configuration, Type type)
        {
            return CreateProxy(configuration, type, null);
        }
        public static T CreateProxy<T>(this IConfiguration configuration)
        {
            return (T)CreateProxy(configuration, typeof(T));
        }
        public static object CreateProxy(this IConfiguration configuration, Type type, INameTransfer nameTransfer)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (nameTransfer is null)
            {
                return ProxyHelper.Default.EnsureCreateProxWithAttribute(type, configuration);
            }
            return ProxyHelper.Default.EnsureCreateProx(type, configuration, nameTransfer);
        }
        public static T CreateProxy<T>(this IConfiguration configuration, INameTransfer nameTransfer)
        {
            return (T)CreateProxy(configuration, typeof(T), nameTransfer);
        }
    }
}
