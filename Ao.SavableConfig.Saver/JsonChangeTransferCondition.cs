using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

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
                var datas = File.ReadAllText(provider.Source.Path);
                var obj = JObject.Parse(datas);
                return new JsonChangeTransfer(obj);
            }
            return null;
        }

        public void Save(ChangeReport report, string transfed)
        {
            if (report.Provider is FileConfigurationProvider provider)
            {
                File.WriteAllText(provider.Source.Path, transfed);
            }
        }
    }
}
