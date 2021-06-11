using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class PropertyCompiledManager : CompiledManager<PropertyInfo, PropertyCompiled>
    {
        protected override PropertyCompiled Compile(PropertyInfo key)
        {
            return new PropertyCompiled(key);
        }
    }
}
