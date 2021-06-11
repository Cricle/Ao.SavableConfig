using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class NullNameTransferTest
    {
        [TestMethod]
        public void GivenPropetyName_ReturnSame()
        {
            var transfer = NullNameTransfer.Instance;
            var val = transfer.Transfer(null, "aa");
            Assert.AreEqual("aa", val);
        }
    }
}
