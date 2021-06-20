using Ao.SavableConfig.ConfigVisit;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Ao.SavableConfig.Yaml
{
    public class YamlConfigurationVisitor : ConfigurationVisitor<ConfigurationKeyPart>
    {
        public YamlConfigurationVisitor(string[] parts,object origin)
            : base(parts)
        {
            Origin = origin;
        }

        public object Origin { get; }

        protected override ConfigurationKeyPart MakeKeyPart(int partIndex)
        {
            return new ConfigurationKeyPart { PartIndex = partIndex };
        }

        protected override bool VisitPart(ConfigurationKeyPart keyPart)
        {
            var isLast = keyPart.PartIndex == (Parts.Length - 1);
            var name = Parts[keyPart.PartIndex];
            if (Origin is Dictionary<string,string>)
            {

            }
        }
    }
}
