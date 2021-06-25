using Ao.SavableConfig.Binder.Visitors;
using Microsoft.Extensions.Configuration;
using System;

namespace Ao.SavableConfig.Binder
{
    public static class ProxHelperExtensions
    {
        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper)
        {
            if (proxyHelper is null)
            {
                throw new ArgumentNullException(nameof(proxyHelper));
            }
            var nameTransfer = IdentityMapNameTransfer.FromTypeAttributes(typeof(T));
            return CreateComplexProxy<T>(proxyHelper, nameTransfer);
        }
        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper, INameTransfer nameTransfer)
        {
            if (proxyHelper is null)
            {
                throw new ArgumentNullException(nameof(proxyHelper));
            }

            if (nameTransfer is null)
            {
                throw new ArgumentNullException(nameof(nameTransfer));
            }

            return CreateComplexProxy<T>(proxyHelper, nameTransfer, IdentityNamedCreator.Instance);
        }
        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper, INameTransfer nameTransfer, INamedCreator namedCreator)
        {
            if (proxyHelper is null)
            {
                throw new ArgumentNullException(nameof(proxyHelper));
            }

            if (nameTransfer is null)
            {
                throw new ArgumentNullException(nameof(nameTransfer));
            }

            if (namedCreator is null)
            {
                throw new ArgumentNullException(nameof(namedCreator));
            }

            return CreateComplexProxy<T>(proxyHelper, nameTransfer, namedCreator, CompilePropertyVisitor.Instance);
        }

        public static ProxyCreator CreateComplexProxy<T>(this ProxyHelper proxyHelper, INameTransfer nameTransfer, INamedCreator namedCreator, IPropertyVisitor propertyVisitor)
        {
            if (proxyHelper is null)
            {
                throw new ArgumentNullException(nameof(proxyHelper));
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

            var creator = new ProxyCreator(proxyHelper, typeof(T), nameTransfer, namedCreator, propertyVisitor);
            creator.Analysis();
            return creator;
        }
        public static T EnsureCreateProxWithAttribute<T>(this ProxyHelper proxHelper, IConfiguration configuration)
            where T : class
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
            where T : class
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
            return (T)proxHelper.CreateProxy(typeof(T), configuration, nameTransfer);
        }
    }
}
