using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class IdentityMapNameTransferTest
    {
        [TestMethod]
        public void GivenNullArgsCall_MustThrowException()
        {
            var transfer = new IdentityMapNameTransfer(new Dictionary<PropertyIdentity, string>());
            Assert.ThrowsException<ArgumentException>(() => transfer.Transfer(new object(),null));
        }
        [TestMethod]
        public void GivenNullInstance_ReturnPropertyName()
        {
            var transfer = new IdentityMapNameTransfer(new Dictionary<PropertyIdentity, string>());
            var val = transfer.Transfer(null, "a");
            Assert.AreEqual("a", val);
        }
    }
}
