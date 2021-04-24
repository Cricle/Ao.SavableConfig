using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Binder
{
    public static class ProxHelperExtensions
    {
        public static T EnsureCreateProxWithAttribute<T>(this ProxyHelper proxHelper, IConfiguration configuration)
            where T:class
        {
            var nameTransfer = IdentityMapNameTransfer.FromTypeAttributes(typeof(T));
            return EnsureCreateProx<T>(proxHelper, configuration, nameTransfer);
        }
        public static T EnsureCreateProx<T>(this ProxyHelper proxHelper,IConfiguration configuration,INameTransfer nameTransfer)
            where T:class
        {
            var type = typeof(T);
            if (!proxHelper.HasTypeProxy(type))
            {
                proxHelper.BuildProx(type);
            }
            return (T)proxHelper.CreateProxy(typeof(T),configuration,nameTransfer);
        }
    }
}
