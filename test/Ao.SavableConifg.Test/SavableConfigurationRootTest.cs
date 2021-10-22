using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Test
{
    [TestClass]
    public class SavableConfigurationRootTest
    {
        [TestMethod]
        public void GivenZeroProvider_EventSubscribe_GetMustThrowException()
        {
            var root = new SavableConfigurationRoot(new IConfigurationProvider[0]);
            root.ConfigurationChanged += (_) => { };
            Assert.ThrowsException<InvalidOperationException>(() => root["a"]="1");
        }
    }
}
