using Ao.ObjectDesign;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class PropertyProxyInfo : IPropertyProxyInfo
    {
        public PropertyIdentity Identity { get; set; }

        public ObjectNamedCreator ParentProxyCreator { get; set; }

        public ObjectNamedCreator ProxyCreator { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public INameTransfer NameTransfer { get; set; }

        public string Key { get; set; }
    }
}
