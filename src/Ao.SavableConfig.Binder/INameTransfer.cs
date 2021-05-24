namespace Ao.SavableConfig.Binder
{
    public interface INameTransfer
    {
        string Transfer(object instance, string propertyName);
    }
}
