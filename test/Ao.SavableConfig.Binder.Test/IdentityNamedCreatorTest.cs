using Ao.SavableConfig.Binder.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class IdentityNamedCreatorTest
    {
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => IdentityNamedCreator.Instance.Create(null, false));
        }
        class ConfigAPart
        {
            public int AP { get; set; }
        }
        class ConfigB
        {
            [ConfigPath("AA")]
            public ConfigAPart A { get; set; }

            [ConfigPath("BB")]
            public int B { get; set; }
        }
        class ConfigA
        {
            [ConfigStepIn]
            [ConfigPath("AA")]
            public ConfigAPart A { get; set; }

            [ConfigPath("BB")]
            public int B { get; set; }
        }
        [TestMethod]
        public void Force_NameTransferMustEqualAttribute()
        {
            var type = typeof(ConfigB);

            var val = IdentityNamedCreator.Instance.Create(type, true);

            Assert.AreEqual(1, val.Count);

            var aProp = val[type.GetProperty(nameof(ConfigB.A))];

            Assert.AreEqual("AA:AP", aProp.Transfer(new ConfigAPart(), "AP"));

            val = IdentityNamedCreator.Instance.Create(type, false);
            Assert.AreEqual(0, val.Count);
        }
        [TestMethod]
        public void NoForce_NameTransferMustEqualAttribute()
        {
            var type = typeof(ConfigA);

            var val = IdentityNamedCreator.Instance.Create(type, false);

            Assert.AreEqual(1, val.Count);

            var aProp = val[type.GetProperty(nameof(ConfigA.A))];

            Assert.AreEqual("AA:AP", aProp.Transfer(new ConfigAPart(), "AP"));
        }
    }
}
