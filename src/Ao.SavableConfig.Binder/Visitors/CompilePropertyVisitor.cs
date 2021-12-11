using Ao.ObjectDesign;
using System.Reflection;

namespace Ao.SavableConfig.Binder.Visitors
{
    public class CompilePropertyVisitor : IPropertyVisitor
    {
        public static readonly CompilePropertyVisitor Instance = new CompilePropertyVisitor();

        private CompilePropertyVisitor() { }
        public object GetValue(object instance, PropertyInfo info)
        {
            var identity=new PropertyIdentity(instance.GetType(),info.Name);
            var getter=CompiledPropertyInfo.GetGetter(identity);
            return getter(instance);
        }

        public void SetValue(object instance, object value, PropertyInfo info)
        {
            var identity = new PropertyIdentity(instance.GetType(), info.Name);
            var setter = CompiledPropertyInfo.GetSetter(identity);
            setter(instance,value);
        }
    }
}
