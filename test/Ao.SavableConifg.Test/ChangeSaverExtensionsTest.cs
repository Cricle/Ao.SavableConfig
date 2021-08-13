using Ao.SavableConfig.Saver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Test
{
    [TestClass]
    public class ChangeSaverExtensionsTest
    {
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ChangeSaverExtensions.EmitAndSave(null));
        }
    }
}
