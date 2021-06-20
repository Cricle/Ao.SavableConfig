using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace Ao.SavableConfig.Yaml
{
    public class YamlChangeTransferCondition : IChangeTransferCondition
    {
        public readonly static YamlChangeTransferCondition Instance = new YamlChangeTransferCondition();

        protected virtual bool IsFileExist(string path)
        {
            return File.Exists(path);
        }
        protected virtual string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
        protected virtual void WriteFile(string path, string datas)
        {
            File.WriteAllText(path, datas);
        }
        public IChangeTransfer GetTransfe(ChangeReport report)
        {
            if (report.Provider is FileConfigurationProvider provider&&
                (string.Equals(Path.GetExtension(provider.Source.Path), ".yaml", System.StringComparison.OrdinalIgnoreCase)||
                string.Equals(Path.GetExtension(provider.Source.Path), ".yml", System.StringComparison.OrdinalIgnoreCase)))
            {
                var path = GetPath(provider.Source);
                if (IsFileExist(path))
                {
                    var datas = ReadFile(path);
                    var reader = new StringReader(datas);
                    var desc = new Deserializer();
                    var obj = JsonNode.Parse(datas);
                    return new JsonChangeTransfer(obj);
                }
                else
                {
                    return new YamlChangeTransfer(null);
                }
            }
            return null;
        }
        private string GetPath(FileConfigurationSource source)
        {
            var path = source.Path;
            if (source.FileProvider is PhysicalFileProvider physicalFileProvider)
            {
                path = Path.Combine(physicalFileProvider.Root, path);
            }
            return path;
        }
        public void Save(ChangeReport report, string transfed)
        {
            if (report.Provider is FileConfigurationProvider provider)
            {
                var path = GetPath(provider.Source);
                WriteFile(path, transfed);
            }
        }
    }
}
