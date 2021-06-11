using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public interface IPropertyProxyInfo
    {
        PropertyIdentity Identity { get; }

        ProxyCreator ParentProxyCreator { get; }

        ProxyCreator ProxyCreator { get; }

        PropertyInfo PropertyInfo { get; }

        INameTransfer NameTransfer { get;  }

        string Key { get; }
    }
}
