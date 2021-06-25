using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ao.SavableConfig.Test
{
    [TestClass]
    public class EmptyChangeWatcherTest
    {
        [TestMethod]
        public void InitWithNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new EmptyChangeWatcher(null));
        }
        [TestMethod]
        public void CallChangeInfos_MustThrowException()
        {
            var root = new SavableConfigurationRoot(new IConfigurationProvider[0]);
            Assert.ThrowsException<NotSupportedException>(() => new EmptyChangeWatcher(root).ChangeInfos);
        }
        [TestMethod]
        public void ConfigurationMutEqualInput_MustThrowException()
        {
            var root = new SavableConfigurationRoot(new IConfigurationProvider[0]);
            var watcher = new EmptyChangeWatcher(root);
            Assert.AreEqual(root, watcher.Configuration);
            watcher.Dispose();
        }
        [TestMethod]
        public void AddChanged_EventMustBeFired()
        {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection();
            var root = builder.BuildSavable();
            var watcher = new EmptyChangeWatcher(root);
            root["aa"] = "qaq";
            IConfigurationChangeInfo info = null;
            watcher.ChangePushed += (o, e) =>
            {
                info = e;
            };
            root["aa"] = "bb";
            Assert.IsNotNull(info);
            watcher.Dispose();
        }
    }
}
