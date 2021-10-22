using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Ao.SavableConfig.Test
{
    [TestClass]
    public class ConfigurationExtensionsTest
    {
        [TestMethod]
        public void CallCreateWatcherWithNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ConfigurationExtensions.CreateWatcher((IConfiguration)null));
            Assert.ThrowsException<ArgumentNullException>(() => ConfigurationExtensions.CreateWatcher(null));
            Assert.ThrowsException<ArgumentNullException>(() => ConfigurationExtensions.CreateEmptyWatcher((IConfiguration)null));
            Assert.ThrowsException<ArgumentNullException>(() => ConfigurationExtensions.CreateEmptyWatcher(null));
            Assert.ThrowsException<ArgumentNullException>(() => ConfigurationExtensions.BuildSavable(null));
        }
        [TestMethod]
        public void CallBuildSavable_MustReturnSavableRoot()
        {
            var builder = new ConfigurationBuilder();
            var root = builder.BuildSavable();
            Assert.IsInstanceOfType(root, typeof(SavableConfigurationRoot));
        }
        [TestMethod]
        public void CallCreateWatcherWithNotSavableConfig_MustThrowException()
        {
            var config = new ConfigurationRoot(new IConfigurationProvider[0]);
            Assert.ThrowsException<InvalidCastException>(() => ConfigurationExtensions.CreateWatcher(config));
        }
        [TestMethod]
        public void CallCreateEmptyWatcherWithNotSavableConfig_MustThrowException()
        {
            var config = new ConfigurationRoot(new IConfigurationProvider[0]);
            Assert.ThrowsException<InvalidCastException>(() => ConfigurationExtensions.CreateEmptyWatcher(config));
        }
        [TestMethod]
        public void CallCreateWatcherWithSavableConfig_MustReturnWatcher()
        {
            var config = new SavableConfigurationRoot(new IConfigurationProvider[0]);
            var watcher = ConfigurationExtensions.CreateWatcher(config);
            Assert.IsNotNull(watcher);
            watcher = ConfigurationExtensions.CreateWatcher((IConfiguration)config);
            Assert.IsNotNull(watcher);
        }
        [TestMethod]
        public void CallCreateEmptyWatcherWithSavableConfig_MustReturnWatcher()
        {
            var config = new SavableConfigurationRoot(new IConfigurationProvider[0]);
            var watcher = ConfigurationExtensions.CreateEmptyWatcher(config);
            Assert.IsNotNull(watcher);
            watcher = ConfigurationExtensions.CreateEmptyWatcher((IConfiguration)config);
            Assert.IsNotNull(watcher);
        }
    }
}
