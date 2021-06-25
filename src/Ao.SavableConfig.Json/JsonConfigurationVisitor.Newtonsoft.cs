using Ao.SavableConfig.ConfigVisit;
using Newtonsoft.Json.Linq;

namespace Ao.SavableConfig.Saver
{
    public class JsonConfigurationVisitor : ConfigurationVisitor<ConfigurationKeyPart>
    {
        public JsonConfigurationVisitor(string[] parts, JToken origin, JToken value)
            : base(parts)
        {
            originTk = origin;
            Value = value;
        }
        private readonly JToken originTk;
        private JToken origin;

        public JToken Value { get; }

        public bool IgnoreAdd { get; set; }

        public override bool VisitWrite()
        {
            origin = originTk;
            return base.VisitWrite();
        }
        protected override ConfigurationKeyPart MakeKeyPart(int partIndex)
        {
            return new ConfigurationKeyPart
            {
                PartIndex = partIndex
            };
        }

        protected override bool VisitPart(ConfigurationKeyPart keyPart)
        {
            var isLast = keyPart.PartIndex == (Parts.Length - 1);
            var name = Parts[keyPart.PartIndex];
            if (origin.Type == JTokenType.Object)
            {
                if (isLast)
                {
                    origin[name] = Value;
                }
                else
                {
                    var objTk = origin[name];
                    if (objTk is null)
                    {
                        if (!IgnoreAdd)
                        {
                            var next = Parts[keyPart.PartIndex + 1];
                            JToken ctk = null;
                            if (int.TryParse(next, out _))
                            {
                                ctk = new JArray();
                            }
                            else
                            {
                                ctk = new JObject();
                            }
                            origin[name] = ctk;
                            origin = ctk;
                        }
                        else
                        {
                            origin = new JValue((object)null);
                        }
                    }
                    else
                    {
                        origin = origin[name];
                    }
                }
            }
            else if (origin.Type == JTokenType.Array)
            {
                var arr = (JArray)origin;
                if (!int.TryParse(name, out var index))
                {
                    return false;
                }
                for (int j = arr.Count; j < index + 1; j++)
                {
                    arr.Add(null);
                }
                if (isLast)
                {
                    arr[index] = Value;
                }
                else
                {
                    var val = arr[index];
                    if (val is null || val.Type == JTokenType.Null)
                    {
                        if (!IgnoreAdd)
                        {
                            var next = Parts[keyPart.PartIndex + 1];
                            if (int.TryParse(next, out _))
                            {
                                origin = new JArray();
                            }
                            else
                            {
                                origin = new JObject();
                            }
                            arr[index] = origin;
                        }
                        else
                        {
                            origin = JObject.FromObject(null);
                        }
                    }
                    else
                    {
                        origin = val;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
