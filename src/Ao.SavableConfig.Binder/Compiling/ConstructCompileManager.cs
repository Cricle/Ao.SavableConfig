using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class ConstructCompileManager : CompiledManager<ConstructorInfo, ConstructCompiled>
    {
        protected override ConstructCompiled Compile(ConstructorInfo key)
        {
            return new ConstructCompiled(key);
        }
    }
}
