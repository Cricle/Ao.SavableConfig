namespace Ao.SavableConfig.Binder
{
    public class PropertyCompiledManager : CompiledManager<PropertyIdentity, PropertyCompiled>
    {
        protected override PropertyCompiled Compile(PropertyIdentity key)
        {
            var prop = key.Type.GetProperty(key.PropertyName);
            return new PropertyCompiled(key.Type, prop);
        }
    }
}
