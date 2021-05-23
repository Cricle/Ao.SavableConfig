using Ao.SavableConfig.Binder.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class ProxyCreatorTest
    {
        public class RedirectBox
        {
            [ConfigStepIn]
            [ConfigPath("P")]
            public RedirectClass Red { get; set; }
        }
        public class NullPathRedirectBox
        {
            [ConfigStepIn]
            public RedirectClass Red { get; set; }
        }
        public class RedirectClass
        {
            public virtual int Age { get; set; }

            public virtual string Name { get; set; }

            [ConfigPath]
            public virtual double Score { get; set; }

            public virtual bool Ok { get; set; }
        }
        [TestMethod]
        public void CreateWithNullMap_MustEqualDefault()
        {
            var prox = ProxyUtil.CreateProx();
            var type = typeof(object);
            var creator = new ProxyCreator(prox, type, null);
            Assert.IsNotNull(creator.NameTransferPicker);
        }
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            var prox = ProxyUtil.CreateProx();
            var type = typeof(object);
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyCreator(prox, null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyCreator(null,type, null));
        }
        [TestMethod]
        [DataRow(typeof(RedirectBox), "P:")]
        [DataRow(typeof(NullPathRedirectBox), "")]
        public void CreateWithRedirectPath_MustProxThatPaths(Type type,string prefx)
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var prox = ProxyUtil.CreateProx();
            var map = new Dictionary<Type, INameTransfer>
            {
                [typeof(RedirectClass)] = new IdentityMapNameTransfer(new Dictionary<PropertyIdentity, string>
                {
                    [new PropertyIdentity(typeof(int), nameof(RedirectClass.Age))] = "Age",
                    [new PropertyIdentity(typeof(string), nameof(RedirectClass.Name))] = prefx+"Name",
                    [new PropertyIdentity(typeof(double), nameof(RedirectClass.Score))] = prefx +"Score",
                })
            };
            var creator = new ProxyCreator(prox, type,map);
            Assert.AreEqual(map, creator.NameTransferPicker);
            var x = (dynamic)creator.Build(root);
            x.Red.Age = 123;
            x.Red.Name = "456";
            x.Red.Score = 789.123d;
            root["Age"] = "123";
            root[prefx+"Name"] = "456";
            root[prefx+"Score"] = "789.123";
            Assert.AreEqual("123",root["Age"]);
            Assert.AreEqual("456", root[prefx+"Name"]);
            Assert.AreEqual("789.123", root[prefx+"Score"]);
        }
        [TestMethod]
        public void GivenArgsInit_PopertyValueMustEqualInput()
        {
            var prox = ProxyUtil.CreateProx();
            var type = typeof(object);
            var dic = new Dictionary<Type, INameTransfer>();
            var creator = new ProxyCreator(prox, type,dic);
            Assert.AreEqual(prox, creator.ProxyHelper);
            Assert.AreEqual(type, creator.Type);
            Assert.AreEqual(dic, creator.NameTransferPicker);
        }
    }
}
