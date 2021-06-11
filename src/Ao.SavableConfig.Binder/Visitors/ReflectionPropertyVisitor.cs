using System.Reflection;

namespace Ao.SavableConfig.Binder.Visitors
{
    public class ReflectionPropertyVisitor : IPropertyVisitor
    {
        public static readonly ReflectionPropertyVisitor Instance = new ReflectionPropertyVisitor();

        public object GetValue(object instance, PropertyInfo info)
        {
            return info.GetValue(instance);
        }

        public void SetValue(object instance, object value, PropertyInfo info)
        {
            info.SetValue(instance, value);
        }
    }
}
