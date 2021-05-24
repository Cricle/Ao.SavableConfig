namespace Ao.SavableConfig.Binder
{
    public class NullNameTransfer : INameTransfer
    {
        public static readonly NullNameTransfer Instance = new NullNameTransfer();
        public string Transfer(object instance, string propertyName)
        {
            return propertyName;
        }
    }
}
