using Microsoft.VisualStudio.TestTools.UnitTesting;

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
