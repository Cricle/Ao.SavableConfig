using Ao.SavableConfig.Saver;
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
    public class ChangeValueInfoTest
    {
        [TestMethod]
        public void New()
        {
            var config = new ConfigurationBuilder().Build();
            var changeInfo = new ConfigurationChangeInfo();
            var val = new ChangeValueInfo(config, changeInfo, ConfigurationTypes.Array,true);

            Assert.AreEqual(config, val.Configuration);
            Assert.AreEqual(changeInfo, val.Info);
            Assert.AreEqual(ConfigurationTypes.Array, val.TypeCode);
            Assert.IsTrue(val.IsArray);
        }
    }
}
