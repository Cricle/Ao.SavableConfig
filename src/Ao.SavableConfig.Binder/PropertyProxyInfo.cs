using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class PropertyProxyInfo : IPropertyProxyInfo
    {
        public PropertyIdentity Identity { get; set; }

        public ProxyCreator ParentProxyCreator { get; set; }

        public ProxyCreator ProxyCreator { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public INameTransfer NameTransfer { get; set; }

        public string Key { get; set; }
    }
}
