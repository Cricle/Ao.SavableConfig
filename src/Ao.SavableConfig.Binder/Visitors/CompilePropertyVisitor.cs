using System.Reflection;

namespace Ao.SavableConfig.Binder.Visitors
{
    public class CompilePropertyVisitor : IPropertyVisitor
    {
        public static readonly CompilePropertyVisitor Instance = new CompilePropertyVisitor();

        private readonly PropertyCompiledManager propertyCompiledManager;

        public CompilePropertyVisitor()
        {
            propertyCompiledManager = new PropertyCompiledManager();
        }

        public object GetValue(object instance, PropertyInfo info)
        {
            var c = propertyCompiledManager.EnsureGetCompiled(info);
            return c.Getter(instance);
        }

        public void SetValue(object instance, object value, PropertyInfo info)
        {
            var c = propertyCompiledManager.EnsureGetCompiled(info);
            c.Setter(instance, value);
        }
    }
}
