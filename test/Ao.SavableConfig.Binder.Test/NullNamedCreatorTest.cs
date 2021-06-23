using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
