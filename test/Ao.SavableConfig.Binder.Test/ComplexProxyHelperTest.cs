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
    public class ComplexProxyHelperTest
    {
        public class ComplexClass
        {
            [ConfigStepIn]
            public NullClass Val { get; set; }
        }
        public class NullClass
        {
            public virtual int Age { get; set; }

            public virtual string Name { get; set; }
        }
        [TestMethod]
        public void GetTwiceDefault_MustEqual()
        {
            var a = ComplexProxyHelper.Default;
            var b = ComplexProxyHelper.Default;
            Assert.AreEqual(a, b);
        }
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ComplexProxyHelper(null));
        }
        [TestMethod]
        public void BuildComplexProxy()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var prox = ProxyUtil.CreateProx();
            var cp = new ComplexProxyHelper(prox, true);
            Assert.IsTrue(cp.AutoAnalysis);
            Assert.AreEqual(prox, cp.ProxyHelper);
            var cr = cp.GetCreatorOrDefault<ComplexClass>();
            Assert.IsNull(cr);
            var c=cp.Build<ComplexClass>(root);
            Assert.IsNotNull(c);
            _ = cp.GetCreatorOrDefault<ComplexClass>();
            Assert.IsNotNull(c);
            Assert.IsTrue(cp.IsCreated<ComplexClass>());
        }
    }
}
