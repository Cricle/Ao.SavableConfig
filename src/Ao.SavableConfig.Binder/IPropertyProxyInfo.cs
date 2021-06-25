using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public interface IPropertyProxyInfo
    {
        PropertyIdentity Identity { get; }

        ObjectNamedCreator ParentProxyCreator { get; }

        ObjectNamedCreator ProxyCreator { get; }

        PropertyInfo PropertyInfo { get; }

        INameTransfer NameTransfer { get; }

        string Key { get; }
    }
}
