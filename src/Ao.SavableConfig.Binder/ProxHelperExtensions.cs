using Ao.ObjectDesign;
using Ao.SavableConfig.Binder.Visitors;
using Microsoft.Extensions.Configuration;
using System;

namespace Ao.SavableConfig.Binder
{
    public static class ProxHelperExtensions
    {
        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper)
            where T : class
        {
            return CreateComplexProxy(proxyHelper, typeof(T));
        }
        public static ProxyCreator CreateComplexProxy(this ProxyHelper proxyHelper, Type type)
        {
            if (proxyHelper is null)
            {
                throw new ArgumentNullException(nameof(proxyHelper));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var nameTransfer = IdentityMapNameTransfer.FromTypeAttributes(type);
            return CreateComplexProxy(proxyHelper, type, nameTransfer);
        }
        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper, INameTransfer nameTransfer)
            where T : class
        {
            return CreateComplexProxy(proxyHelper, typeof(T), nameTransfer);
        }
        public static ProxyCreator CreateComplexProxy(this ProxyHelper proxyHelper, Type type, INameTransfer nameTransfer)
        {
            return CreateComplexProxy(proxyHelper, type, nameTransfer, IdentityNamedCreator.Instance);
        }
        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper, INameTransfer nameTransfer, INamedCreator namedCreator)
            where T : class
        {
            return CreateComplexProxy(proxyHelper, typeof(T), nameTransfer, namedCreator);
        }
        public static ProxyCreator CreateComplexProxy(this ProxyHelper proxyHelper, Type type, INameTransfer nameTransfer, INamedCreator namedCreator)
        {
            if (proxyHelper is null)
            {
                throw new ArgumentNullException(nameof(proxyHelper));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (nameTransfer is null)
            {
                throw new ArgumentNullException(nameof(nameTransfer));
            }

            if (namedCreator is null)
            {
                throw new ArgumentNullException(nameof(namedCreator));
            }

            return CreateComplexProxy(proxyHelper, type, nameTransfer, namedCreator, CompilePropertyVisitor.Instance);
        }
        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper, INameTransfer nameTransfer, INamedCreator namedCreator, Visitors.IPropertyVisitor propertyVisitor)
            where T : class
        {
            return CreateComplexProxy(proxyHelper, typeof(T), nameTransfer, namedCreator, propertyVisitor);
        }
        public static ProxyCreator CreateComplexProxy(this ProxyHelper proxyHelper, Type type, INameTransfer nameTransfer, INamedCreator namedCreator, Visitors.IPropertyVisitor propertyVisitor)
        {
            if (proxyHelper is null)
            {
                throw new ArgumentNullException(nameof(proxyHelper));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (nameTransfer is null)
            {
                throw new ArgumentNullException(nameof(nameTransfer));
            }

            if (namedCreator is null)
            {
                throw new ArgumentNullException(nameof(namedCreator));
            }

            if (propertyVisitor is null)
            {
                throw new ArgumentNullException(nameof(propertyVisitor));
            }

            var creator = new ProxyCreator(proxyHelper, type, nameTransfer, namedCreator, propertyVisitor);
            creator.Analysis();
            return creator;
        }
        public static T EnsureCreateProxWithAttribute<T>(this ProxyHelper proxHelper, IConfiguration configuration)
            where T : class
        {
            return (T)EnsureCreateProxWithAttribute(proxHelper, typeof(T), configuration);
        }
        public static object EnsureCreateProxWithAttribute(this ProxyHelper proxHelper, Type type, IConfiguration configuration)
        {
            if (proxHelper is null)
            {
                throw new ArgumentNullException(nameof(proxHelper));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var nameTransfer = IdentityMapNameTransfer.FromTypeAttributes(type);
            return EnsureCreateProx(proxHelper, type, configuration, nameTransfer);
        }
        public static T EnsureCreateProx<T>(this ProxyHelper proxHelper, IConfiguration configuration, INameTransfer nameTransfer)
            where T : class
        {
            return (T)EnsureCreateProx(proxHelper, typeof(T), configuration, nameTransfer);
        }
        public static object EnsureCreateProx(this ProxyHelper proxHelper, Type type, IConfiguration configuration, INameTransfer nameTransfer)
        {
            if (proxHelper is null)
            {
                throw new ArgumentNullException(nameof(proxHelper));
            }

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

            if (!proxHelper.HasTypeProxy(type))
            {
                proxHelper.BuildProx(type);
            }
            return proxHelper.CreateProxy(type, configuration, nameTransfer);
        }
    }
}
