using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
