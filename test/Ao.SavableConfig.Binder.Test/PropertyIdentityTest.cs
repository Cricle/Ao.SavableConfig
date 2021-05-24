using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class PropertyIdentityTest
    {
        [TestMethod]
        public void GivenNullInit_MuthThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PropertyIdentity(null, "a"));
            Assert.ThrowsException<ArgumentException>(() => new PropertyIdentity(typeof(object),null));
        }
        [TestMethod]
        public void GivenSameProperyIdentity_MustEqual()
        {
            var pi1 = new PropertyIdentity(typeof(object), "a");
            var pi2 = new PropertyIdentity(typeof(object), "a");
            Assert.AreEqual(pi1, pi2);
            Assert.AreEqual(pi1.GetHashCode(), pi2.GetHashCode());
            Assert.IsTrue(pi1.Equals(pi2));
            pi1.ToString();
        }
        [TestMethod]
        public void GivenNotSameProperyIdentity_MustNotEqual()
        {
            var pi1 = new PropertyIdentity(typeof(object), "a");
            var pi2 = new PropertyIdentity(typeof(int), "a");
            Assert.AreNotEqual(pi1, pi2);
            Assert.AreNotEqual(pi1.GetHashCode(), pi2.GetHashCode());

            pi1 = new PropertyIdentity(typeof(object), "a");
            pi2 = new PropertyIdentity(typeof(object), "b");
            Assert.AreNotEqual(pi1, pi2);
            Assert.AreNotEqual(pi1.GetHashCode(), pi2.GetHashCode());

            pi1 = new PropertyIdentity(typeof(int), "q");
            pi2 = new PropertyIdentity(typeof(object), "b");
            Assert.AreNotEqual(pi1, pi2);
            Assert.AreNotEqual(pi1.GetHashCode(), pi2.GetHashCode());

        }
    }
}
