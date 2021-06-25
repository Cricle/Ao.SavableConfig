using System;
using System.Text.Json.Node;

namespace Ao.SavableConfig.Saver
{
    public partial class JsonChangeTransfer : IChangeTransfer
    {
        public JsonChangeTransfer(JsonNode origin)
        {
            Origin = origin;
        }

        public JsonNode Origin { get; }

        public string Transfe(ChangeReport report)
        {
            var tk = JsonNode.Parse(Origin.ToJsonString());
            foreach (var item in report.IncludeChangeInfo)
            {
                var jtoken = item.Key.Split(splitToken, StringSplitOptions.RemoveEmptyEntries);
                var visitor = new JsonConfigurationVisitor(jtoken, tk, item.New);
                visitor.IgnoreAdd = IgnoreAdd;
                visitor.VisitWrite();
            }
            return tk.ToJsonString();
        }
    }
}
