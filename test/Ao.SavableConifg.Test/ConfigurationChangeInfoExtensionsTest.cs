using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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
            var ud = ConfigurationChangeInfoExtensions.CreateUndo(info);
            Assert.AreEqual("a", ud.Old);
            Assert.AreEqual("b", ud.New);
        }
#if NET5_0
        [TestMethod]
        public void Undo_ValueMustSet()
        {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection();
            var root = builder.BuildSavable();
            var info = new ConfigurationChangeInfo
            {
                Key = "hello",
                New = "a",
                Old = "b",
                Provider = root.Providers.First()
            };
            ConfigurationChangeInfoExtensions.Undo(info);
            var val = root["hello"];
            Assert.AreEqual("b", val);
        }
#endif
    }
}
