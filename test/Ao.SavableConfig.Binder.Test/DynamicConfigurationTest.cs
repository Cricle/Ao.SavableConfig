using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
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

            var a = dy.a();
            Assert.IsNull(a);
            dy.a = "dw123";
            a = dy.a();
            Assert.AreEqual("dw123", a);
            Assert.AreEqual("dw123", section["a"]);

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
        [TestMethod]
        public void GetCaseNull_ReturnNull()
        {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection();

            var root = builder.Build();
            var val = root.FastGetValue<BindBox>("aaa");

            Assert.IsNull(val);
        }
        [TestMethod]
        public void Foreach()
        {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("a:0","1"),
                new KeyValuePair<string, string>("a:1","2"),
                new KeyValuePair<string, string>("a:2","3"),
            });

            var root = builder.Build();
            var dyn = root.CreateDynamic();

            var enu = (IEnumerable< IConfigurationSection>)dyn.a;

            Assert.IsNotNull(enu);

            foreach (var item in enu)
            {
            }

            foreach (var item in dyn.a)
            {
                Assert.IsInstanceOfType(item, typeof(IConfigurationSection));
            }
        }
    }
}
