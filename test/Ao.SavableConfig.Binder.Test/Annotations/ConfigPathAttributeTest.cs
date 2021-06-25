using Ao.SavableConfig.Binder.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ao.SavableConfig.Binder.Test.Annotations
{
    [TestClass]
    public class ConfigPathAttributeTest
    {
        [TestMethod]
        public void InitEmpty()
        {
            new ConfigPathAttribute { Absolute = true };
        }
        [TestMethod]
        public void GivenAnySegements_MustCombined()
        {
            var args = new string[] { "a", "b", "c" };
            var exp = ConfigurationPath.Combine(args);
            var p = new ConfigPathAttribute(args);
            Assert.AreEqual(exp, p.Name);
        }
    }
}
