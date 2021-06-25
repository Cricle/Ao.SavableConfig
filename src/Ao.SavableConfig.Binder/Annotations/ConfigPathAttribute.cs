using Microsoft.Extensions.Configuration;
using System;

namespace Ao.SavableConfig.Binder.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigPathAttribute : Attribute
    {
        public ConfigPathAttribute()
        {
        }

        public ConfigPathAttribute(params string[] parts)
        {
            Name = ConfigurationPath.Combine(parts);
        }
        public ConfigPathAttribute(string name)
        {
            Name = name;
        }
        public bool Absolute { get; set; }

        public string Name { get; }
    }
}
