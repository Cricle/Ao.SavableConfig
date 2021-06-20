using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace Ao.SavableConfig.Yaml
{
    public class YamlChangeTransfer : IChangeTransfer
    {
        private static readonly string[] splitToken = new string[] { ConfigurationPath.KeyDelimiter };

        public string OriginContent { get; }

        public IDeserializer Deserializer { get; }

        public ISerializer Serializer { get; }

        public object Origin { get; }

        public string Transfe(ChangeReport report)
        {
            var copy = Deserializer.Deserialize(OriginContent);
            foreach (var item in report.IncludeChangeInfo)
            {
                var jtoken = item.Key.Split(splitToken, StringSplitOptions.RemoveEmptyEntries);
                var visitor = new YamlConfigurationVisitor(jtoken, tk, item.New);
                visitor.VisitWrite();
            }
            return Serializer.Serialize(Origin);

        }
    }
}
