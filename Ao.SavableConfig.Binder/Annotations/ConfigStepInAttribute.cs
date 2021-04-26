using System;
using System.Collections.Generic;
using System.Text;

namespace Ao.SavableConfig.Binder.Annotations
{
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigStepInAttribute : Attribute
    {
    }
}
