using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Ao.SavableConfig.ConfigVisit;

namespace Ao.SavableConfig.Saver
{
    public partial class JsonChangeTransfer : IChangeTransfer
    {
        public JsonChangeTransfer(JToken origin)
        {
            Origin = origin;
        }

        public JToken Origin { get; }

        public string Transfe(ChangeReport report)
        {
            var tk = Origin.DeepClone();
            foreach (var item in report.IncludeChangeInfo)
            {
                var jtoken = item.Key.Split(splitToken, StringSplitOptions.RemoveEmptyEntries);
                var visitor = new JsonConfigurationVisitor(jtoken,tk,item.New);
                visitor.IgnoreAdd = IgnoreAdd;
                visitor.VisitWrite();
            }
            return tk.ToString();
        }
    }
}
