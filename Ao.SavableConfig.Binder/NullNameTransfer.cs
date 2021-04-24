namespace Ao.SavableConfig.Binder
{
    public class NullNameTransfer : INameTransfer
    {
        public string Transfer(object instance, string propertyName)
        {
            return propertyName;
        }
    }
}
