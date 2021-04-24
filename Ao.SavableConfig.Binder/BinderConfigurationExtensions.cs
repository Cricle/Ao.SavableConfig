using Microsoft.Extensions.Configuration;
using System;

namespace Ao.SavableConfig.Binder
{
    public static class BinderConfigurationExtensions
    {
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
