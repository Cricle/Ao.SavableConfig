using System;
using System.Collections.Generic;
using System.Text;

namespace Ao.SavableConfig.Binder.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigStepInAttribute : Attribute
    {
    }
}
