using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Ao.SavableConfig.Saver
{
    public class JsonChangeTransfer : IChangeTransfer
    {
        private static readonly string[] splitToken= new string[] { ConfigurationPath.KeyDelimiter };
        public JsonChangeTransfer(JToken origin)
        {
            Origin = origin;
        }

        public JToken Origin { get; }

        public bool IgnoreAdd { get; set; }

        protected internal void UpdateValue(JToken origin,string path,JToken value)
        {
            var tk = origin;
            var jtoken = path.Split(splitToken, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < jtoken.Length; i++)
            {
                var isLast = i == (jtoken.Length - 1);
                var name = jtoken[i];
                if (tk.Type == JTokenType.Object)
                {
                    if (isLast)
                    {
                        tk[name] = value;
                    }
                    else
                    {
                        var objTk =tk[name];
                        if (objTk is null)
                        {
                            if (!IgnoreAdd)
                            {
                                var next = jtoken[i + 1];
                                JToken ctk = null;
                                if (int.TryParse(next, out _))
                                {
                                    ctk = new JArray();
                                }
                                else
                                {
                                    ctk = new JObject();
                                }
                                tk[name] = ctk;
                                tk = ctk;
                            }
                            else
                            {
                                tk = JToken.FromObject(null);
                            }
                        }
                        else
                        {
                            tk = tk[name];
                        }
                    }
                }
                else if (tk.Type == JTokenType.Array)
                {
                    var arr = (JArray)tk;
                    if (!int.TryParse(name,out var index))
                    {
                        return;
                    }
                    for (int j = arr.Count; j < index + 1; j++)
                    {
                        arr.Add(null);
                    }
                    if (isLast)
                    {
                        arr[index] = value;
                    }
                    else
                    {
                        var val = arr[index];
                        if (val is null||val.Type== JTokenType.Null)
                        {
                            if (!IgnoreAdd)
                            {
                                var next = jtoken[i + 1];
                                if (int.TryParse(next, out _))
                                {
                                    tk = new JArray();
                                }
                                else
                                {
                                    tk = new JObject();
                                }
                                arr[index] = tk;
                            }
                            else
                            {
                                tk = JObject.FromObject(null);
                            }
                        }
                        else
                        {
                            tk = val;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }
        public string Transfe(ChangeReport report)
        {
            var tk = Origin.DeepClone();
            foreach (var item in report.IncludeChangeInfo)
            {
                UpdateValue(tk, item.Key, item.New);
            }
            return tk.ToString();
        }
    }
}
