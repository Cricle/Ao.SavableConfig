using Ao.SavableConfig.ConfigVisit;
using System.Text.Json;
using System.Text.Json.Node;

namespace Ao.SavableConfig.Saver
{
    public class JsonConfigurationVisitor : ConfigurationVisitor<ConfigurationKeyPart>
    {
        public JsonConfigurationVisitor(string[] parts, JsonNode origin, JsonNode value)
            : base(parts)
        {
            originTk = origin;
            Value = value;
        }
        private readonly JsonNode originTk;
        private JsonNode origin;

        public JsonNode Value { get; }

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
            if (origin is JsonObject)
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
                            JsonNode ctk = null;
                            if (int.TryParse(next, out _))
                            {
                                ctk = new JsonArray();
                            }
                            else
                            {
                                ctk = new JsonObject();
                            }
                            origin[name] = ctk;
                            origin = ctk;
                        }
                        else
                        {
                            origin = JsonValue.Create((object)null);
                        }
                    }
                    else
                    {
                        origin = origin[name];
                    }
                }
            }
            else if (origin is JsonArray)
            {
                var arr = (JsonArray)origin;
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
                    if (val is null || val.GetValue<string>() == null)
                    {
                        if (!IgnoreAdd)
                        {
                            var next = Parts[keyPart.PartIndex + 1];
                            if (int.TryParse(next, out _))
                            {
                                origin = new JsonArray();
                            }
                            else
                            {
                                origin = new JsonObject();
                            }
                            arr[index] = origin;
                        }
                        else
                        {
                            origin = JsonValue.Create<string>(null);
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
