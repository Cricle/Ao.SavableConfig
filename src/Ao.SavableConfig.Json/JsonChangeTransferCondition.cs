using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Ao.SavableConfig.Saver
{
    public partial class JsonChangeTransferCondition : IChangeTransferCondition
    {
        public readonly static JsonChangeTransferCondition Instance = new JsonChangeTransferCondition();

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
