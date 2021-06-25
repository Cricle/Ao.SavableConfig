using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class NullNamedCreatorTest
    {
        [TestMethod]
        public void Create_MustEmptyMap()
        {
            var a = new NullNamedCreator();
            Assert.AreEqual(0, a.Create(null, false).Count);

            a = NullNamedCreator.Instance;
            Assert.AreEqual(0, a.Create(null, false).Count);
        }
    }
}
