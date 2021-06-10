using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Ao.SavableConfig.Saver
{
    public partial class JsonChangeTransferCondition : IChangeTransferCondition
    {
        public IChangeTransfer GetTransfe(ChangeReport report)
        {
            if (report.Provider is FileConfigurationProvider provider
                && string.Equals(Path.GetExtension(provider.Source.Path), ".json", System.StringComparison.OrdinalIgnoreCase))
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
    }
}
