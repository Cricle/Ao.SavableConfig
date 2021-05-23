using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Test
{
    [TestClass]
    public class ConfigurationChangeInfoExtensionsTest
    {
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ConfigurationChangeInfoExtensions.CreateUndo(null));
            Assert.ThrowsException<ArgumentNullException>(() => ConfigurationChangeInfoExtensions.Undo(null));
        }
        [TestMethod]
        public void CreateUndo_NewAndOldMustBeRev()
        {
            var info = new ConfigurationChangeInfo
            {
                New = "a",
                Old = "b"
            };
            var ud= ConfigurationChangeInfoExtensions.CreateUndo(info);
            Assert.AreEqual("a", ud.Old);
            Assert.AreEqual("b", ud.New);
        }
        [TestMethod]
        public void Undo_ValueMustSet()
        {
            var builder = new SavableConfiurationBuilder();
            builder.AddInMemoryCollection();
            var root = builder.Build();
            var info = new ConfigurationChangeInfo
            {
                Key="hello",
                New = "a",
                Old = "b",
                Provider = root.Providers.First()
            };
            ConfigurationChangeInfoExtensions.Undo(info);
            var val = root["hello"];
            Assert.AreEqual("b", val);
        }
    }
}
