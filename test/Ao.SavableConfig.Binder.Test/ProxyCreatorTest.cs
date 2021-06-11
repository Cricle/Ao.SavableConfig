using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Binder.Visitors;
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
        public void GivenNullInit_MustThrowException()
        {
            var prox = ProxyUtil.CreateProx();
            var type = typeof(object);
            var nameTransfer = NullNameTransfer.Instance;
            var nameCreator = IdentityNamedCreator.Instance;
            var propVisitor = CompilePropertyVisitor.Instance;

            Assert.ThrowsException<ArgumentNullException>(() => new ProxyCreator(null, type, nameTransfer, nameCreator, propVisitor));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyCreator(prox, null, nameTransfer, nameCreator, propVisitor));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyCreator(prox, type, null, nameCreator, propVisitor));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyCreator(prox, type, nameTransfer, null, propVisitor));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyCreator(prox, type, nameTransfer, nameCreator, null));
        }
        [TestMethod]
        [DataRow(typeof(RedirectBox), "P:")]
        [DataRow(typeof(NullPathRedirectBox), "")]
        public void CreateWithRedirectPath_MustProxThatPaths(Type type,string prefx)
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var prox = ProxyUtil.CreateProx();
            var named = new Dictionary<PropertyIdentity, string>
            {
                [new PropertyIdentity(typeof(int), nameof(RedirectClass.Age))] = "Age",
                [new PropertyIdentity(typeof(string), nameof(RedirectClass.Name))] = prefx + "Name",
                [new PropertyIdentity(typeof(double), nameof(RedirectClass.Score))] = prefx + "Score",
            };
            var map = new IdentityMapNameTransfer(named);
            var creator = new ProxyCreator(prox, type,map,IdentityNamedCreator.Instance,ReflectionPropertyVisitor.Instance);

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
            var nameTransfer = NullNameTransfer.Instance;
            var nameCreator = IdentityNamedCreator.Instance;
            var propVisitor = CompilePropertyVisitor.Instance;
            var creator = new ProxyCreator(prox, type, nameTransfer, nameCreator, propVisitor);
            Assert.AreEqual(prox, creator.ProxyHelper);
            Assert.AreEqual(type, creator.Type);
            Assert.AreEqual(nameTransfer, creator.NameTransfer);
            Assert.AreEqual(propVisitor, creator.PropertyVisitor);
            Assert.AreEqual(nameCreator, creator.NamedCreator);
        }
    }
}
