using Ao.ObjectDesign;
using Ao.SavableConfig.Binder.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class IdentityMapNameTransferTest
    {
        [TestMethod]
        public void GivenNullArgsCall_MustThrowException()
        {
            var transfer = new IdentityMapNameTransfer(new Dictionary<PropertyIdentity, string>());
            Assert.ThrowsException<ArgumentException>(() => transfer.Transfer(new object(), null));
            Assert.ThrowsException<ArgumentNullException>(() => new IdentityMapNameTransfer(null));
        }
        [TestMethod]
        public void GivenNullInstance_ReturnPropertyName()
        {
            var transfer = new IdentityMapNameTransfer(new Dictionary<PropertyIdentity, string>());
            var val = transfer.Transfer(null, "a");
            Assert.AreEqual("a", val);
        }
        class ABox
        {
            public string A { get; set; }
        }
        [TestMethod]
        public void GivenBasePath_TransterIt_ReturnMustCombinedBasePath()
        {
            var basePath = "BasePath";
            var map = new Dictionary<PropertyIdentity, string>
            {
                [new PropertyIdentity(typeof(ABox), "A")] = "B"
            };
            var transfer = new IdentityMapNameTransfer(basePath, map);
            Assert.AreEqual(basePath, transfer.BasePath);
            var n = transfer.Transfer(new ABox(), "A");
            Assert.AreEqual("B", n);
        }
        [TestMethod]
        public void FromTypesBuildTransfer_MustOk()
        {
            var t = IdentityMapNameTransfer.FromTypesAttributes(new Type[] { typeof(object) });
            Assert.AreEqual(1, t.Count);
            Assert.AreEqual(typeof(object), t.Keys.First());
        }
        [ConfigPath("Cl")]
        class Class1
        {
            public virtual int Id { get; set; }
        }
        [TestMethod]
        public void UsingClassPath_MustCombineWithIt()
        {
            var t = IdentityMapNameTransfer.FromTypeAttributes("A", typeof(Class1), true);
            Assert.AreEqual("A:Cl", t.BasePath);
        }
    }
}
