using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class BindBoxTest
    {
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var setting = new BindSettings(null, default, null);
            Assert.ThrowsException<ArgumentNullException>(() => new BindBox(null, setting, ConfigBindMode.TwoWay, x => x()));
            Assert.ThrowsException<ArgumentNullException>(() => new BindBox(root, null, ConfigBindMode.TwoWay, x => x()));
            Assert.ThrowsException<ArgumentNullException>(() => new BindBox(root, setting, ConfigBindMode.TwoWay, null));
        }
    }
}
