using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class BinderExtensionsTest
    {

        public class ComplexClass
        {
            [ConfigStepIn]
            public NullClass Value { get; set; }
        }
        public class NullClass
        {
            public virtual int? Age { get; set; }

            public virtual string Name { get; set; }
        }
        private readonly object locker = new object();
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => BinderExtensions.CreateDynamic(null));

            var root = ConfigHelper.CreateEmptyRoot();
            var setting = new BindSettings(null, default, null);
            Assert.ThrowsException<ArgumentNullException>(() => BinderExtensions.Bind(null, setting, ConfigBindMode.TwoWay));
            Assert.ThrowsException<ArgumentNullException>(() => BinderExtensions.Bind(root, null, ConfigBindMode.TwoWay));

            Assert.ThrowsException<ArgumentNullException>(() => BinderExtensions.BindTwoWay(null, setting));
            Assert.ThrowsException<ArgumentNullException>(() => BinderExtensions.BindTwoWay(root, null));
            Assert.ThrowsException<ArgumentNullException>(() => BinderExtensions.BindTwoWay(root, setting, null));
        }

        [TestMethod]
        public void CreateDynamicConfig_MustOk()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var dy = BinderExtensions.CreateDynamic(root);
            Assert.IsNotNull(dy);
        }
        [TestMethod]
        public void CreateBind_MustBinded()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var val = root.AutoCreateProxy<NullClass>();
            var originVal = new NullClass();
            var setting = new BindSettings(originVal, BindSettings.DefaultDelayTime, new IChangeTransferCondition[0]);
            var box = BinderExtensions.Bind(root, setting, ConfigBindMode.OneTime);
            Assert.AreEqual(ConfigBindMode.OneTime, box.Mode);
            Assert.AreEqual(setting, box.BindSettings);
            Assert.AreEqual(root, box.ChangeNotifyable);
            Assert.IsNotNull(box.Updater);

            box.Bind();
            box.Bind();
            Assert.IsTrue(box.IsBind);
            originVal.Name = "123";
            Assert.IsNull(root["Name"]);
            box.UnBind();
            box.UnBind();
            box.Dispose();
            originVal.Name = null;

            val = root.AutoCreateProxy<NullClass>();
            setting = new BindSettings(val, BindSettings.DefaultDelayTime, new IChangeTransferCondition[0]);
            box = BinderExtensions.Bind(root, setting, ConfigBindMode.OneWayToSource);
            val.Name = "456";
            Assert.AreEqual("456", root["Name"]);
            box.Dispose();
            val.Name = null;

            val = root.AutoCreateProxy<NullClass>();
            setting = new BindSettings(val, BindSettings.DefaultDelayTime, new IChangeTransferCondition[0]);
            box = BinderExtensions.Bind(root, setting, ConfigBindMode.OneWay);
            root["Name"] = "789";
            Assert.AreEqual("789", val.Name);
            box.Dispose();
            val.Name = null;

            val = root.AutoCreateProxy<NullClass>();
            box = BinderExtensions.BindTwoWay(root, val);
            root["Name"] = "789";
            Assert.AreEqual("789", val.Name);
            val.Name = "999";
            Assert.AreEqual("999", root["Name"]);
            BindBox rd = null;
            box.Reloaded += (e) =>
            {
                rd = e;
            };
            root.Reload();
            Assert.AreEqual(box, rd);
            box.Dispose();
        }
    }
}
