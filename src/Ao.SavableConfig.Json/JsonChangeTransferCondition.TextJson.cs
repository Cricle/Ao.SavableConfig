using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json.Node;

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
                    var obj = JsonNode.Parse(datas);
                    return new JsonChangeTransfer(obj);
                }
                else
                {
                    return new JsonChangeTransfer(new JsonObject());
                }
            }
            return null;
        }
    }
}
