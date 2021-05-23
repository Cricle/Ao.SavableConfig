using Ao.SavableConfig.Saver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Test.Saver
{
    [TestClass]
    public class TypeHelperTest
    {
        class Stt
        {
            public Stt(ConfigurationTypes type, string value)
            {
                Type = type;
                Value = value;
            }

            public ConfigurationTypes Type { get; }

            public string Value { get; }
        }
        [TestMethod]
        public void StringToType()
        {
            var map = new Stt[]
            {
                new Stt(ConfigurationTypes.Null,null),
                new Stt(ConfigurationTypes.String,string.Empty),
               new Stt (ConfigurationTypes.String,"yurf3uq2yfeuydfwgci(*&^%$#@"),
               new Stt (ConfigurationTypes.Boolean,"true"),
               new Stt (ConfigurationTypes.Boolean,"false"),
               new Stt (ConfigurationTypes.Number,"123"),
               new Stt (ConfigurationTypes.Number,"0"),
               new Stt (ConfigurationTypes.Number,"-123"),
               new Stt (ConfigurationTypes.Single,"-123.123"),
               new Stt (ConfigurationTypes.Single,"123.123"),
            };
            foreach (var item in map)
            {
                var val = TypeHelper.GetTypeCode(item.Value);
                Assert.AreEqual(item.Type, val);
            }
        }
    }
}
