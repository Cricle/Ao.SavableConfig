using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class BindSettingsTest
    {
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Init_PropertyValueMustGiven(bool includeAction)
        {
            var val = new object();
            var ts = TimeSpan.FromDays(1);
            var conditions = new IChangeTransferCondition[0];
            Action<Action> action = x => x();
            BindSettings setting = null;
            if (includeAction)
            {
                setting = new BindSettings(val, ts, conditions, action);
            }
            else
            {
                setting = new BindSettings(val, ts, conditions);
            }
            Assert.AreEqual(val, setting.Value);
            Assert.AreEqual(ts, setting.DelayTime);
            Assert.AreEqual(conditions, setting.Conditions);
            if (includeAction)
            {
                Assert.AreEqual(action, setting.Updater);
            }
        }
    }
}
