using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Ao.SavableConfig.Saver
{
    public class JsonChangeTransferCondition : IChangeTransferCondition
    {
        public readonly static JsonChangeTransferCondition Instance = new JsonChangeTransferCondition();

        public IChangeTransfer GetTransfe(ChangeReport report)
        {
            if (report.Provider is FileConfigurationProvider provider
                && string.Equals(Path.GetExtension(provider.Source.Path),".json", System.StringComparison.OrdinalIgnoreCase))
            {
                var path = GetPath(provider.Source);
                if (IsFileExist(path))
                {
                    var datas = ReadFile(path);
                    var obj = JObject.Parse(datas);
                    return new JsonChangeTransfer(obj);
                }
                else
                {
                    return new JsonChangeTransfer(new JObject());
                }
            }
            return null;
        }
        protected virtual bool IsFileExist(string path)
        {
            return File.Exists(path);
        }
        protected virtual string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
        protected virtual void WriteFile(string path,string datas)
        {
            File.WriteAllText(path, datas);
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
