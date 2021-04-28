using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Test
{
    [TestClass]
    public class ChangeWatcherTest
    {
        [TestMethod]
        public void Given_NullInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ChangeWatcher(null));
        }
    }
}
