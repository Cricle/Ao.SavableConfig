using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ao.SavableConfig.Binder.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigPathAttribute : Attribute
    {
        public ConfigPathAttribute(params string[] parts)
        {
            if (parts is null)
            {
                throw new ArgumentNullException(nameof(parts));
            }
            Name = ConfigurationPath.Combine(parts);
        }
        public ConfigPathAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"“{nameof(name)}”不能为 Null 或空。", nameof(name));
            }

            Name = name;
        }

        public string Name { get; }
    }
}
