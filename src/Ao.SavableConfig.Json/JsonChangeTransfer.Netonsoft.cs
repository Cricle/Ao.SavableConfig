
/* 项目“Ao.SavableConfig.Json (net452)”的未合并的更改
在此之前:
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
在此之后:
using Ao.SavableConfig.ConfigVisit;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
*/
using Newtonsoft.Json.Linq;
using System
/* 项目“Ao.SavableConfig.Json (net452)”的未合并的更改
在此之前:
using Ao.SavableConfig.ConfigVisit;
在此之后:
using System.Linq;
*/
;

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
                var visitor = new JsonConfigurationVisitor(jtoken, tk, item.New);
                visitor.IgnoreAdd = IgnoreAdd;
                visitor.VisitWrite();
            }
            return tk.ToString();
        }
    }
}
