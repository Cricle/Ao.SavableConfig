using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class DynamicConfigurationTest
    {
        [TestMethod]
        public void CallWithPath_ValueCanGetSet()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var cfg = new DynamicConfiguration(root);
            string changePath = null;
            dynamic dy = cfg;
            var helloSection = dy.hello;
            ((INotifyPropertyChanged)helloSection).PropertyChanged += (o, e) =>
            {
                changePath = e.PropertyName;
            };
            helloSection.world = "123";
            Assert.AreEqual("hello:world", changePath);
            var val = dy.a.b.c();
            Assert.IsNull(val);
            dy.a.b.c = "123";
            val = dy.a.b.c();
            Assert.AreEqual("123", val);
            Assert.AreEqual("123", root["a:b:c"]);

            dy["w:q"] = 123;
            Assert.AreEqual("123", dy["w:q"]);
            Assert.AreEqual("123", dy.w.q());

            dy[1] = 456;
            Assert.AreEqual("456", root["1"]);

            root["qwerty"] = "123";
            var v = dy.qwerty(typeof(int));
            Assert.AreEqual(123, v);
        }
        [TestMethod]
        public void CallWithSection_ValueCanGetSet()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var section = root.GetSection("W");
            dynamic dy = new DynamicConfiguration(section);

            var val = dy.a.b.c();
            Assert.IsNull(val);
            dy.a.b.c = "123";
            val = dy.a.b.c();
            Assert.AreEqual("123", val);
            Assert.AreEqual("123", section["a:b:c"]);

            dy["w:q"] = 123;
            Assert.AreEqual("123", dy["w:q"]);
            Assert.AreEqual("123", dy.w.q());
        }
    }
}
