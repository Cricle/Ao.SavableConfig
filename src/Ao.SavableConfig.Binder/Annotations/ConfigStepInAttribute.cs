using System;

namespace Ao.SavableConfig.Binder.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigStepInAttribute : Attribute
    {
    }
}
