using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class BindTokenTest
    {
        [TestMethod]
        public void Dispose_DisposedMustBeCall()
        {
            var ok = false;
            var tk = new BindToken
            {
                Disposed = () => ok = true
            };
            tk.Dispose();
            Assert.IsTrue(ok);
        }
    }
}
