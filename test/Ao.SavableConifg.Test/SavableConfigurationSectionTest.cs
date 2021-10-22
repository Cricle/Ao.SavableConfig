using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Test
{
    [TestClass]
    public class SavableConfigurationSectionTest
    {
#if NET452

        private SavableConfigurationRoot CreateRoot()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["a"] = "1",
                    ["b"] = "2",
                    ["a:q"]="3",
                    ["a:v"]="3"
                })
                .BuildSavable();
        }
        [TestMethod]
        public void GetSetValue()
        {
            var root = CreateRoot();
            var section = new SavableConfigurationSection(root, "a");
            var a = section["q"];
            Assert.AreEqual("3", a);
            section["q"] = "5";
            Assert.AreEqual("5", root["a:q"]);

            IConfigurationSection s = section;

            a = s["v"];
            Assert.AreEqual("3", a);
            s["v"] = "5";
            Assert.AreEqual("5", root["a:v"]);

        }
        [TestMethod]
        public void GetSection()
        {
            var root = CreateRoot();
            var section = new SavableConfigurationSection(root, "a");
            var s2 = (SavableConfigurationSection)section.GetSection("r");
            s2.Value = "123";
            Assert.AreEqual("123", s2.Value);


            var s3 = ((IConfigurationSection)section).GetSection("t");
            s3.Value = "123";
            Assert.AreEqual("123", s3.Value);
        }
#endif
    }
}
