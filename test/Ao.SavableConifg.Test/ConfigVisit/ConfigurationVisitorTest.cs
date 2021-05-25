using Ao.SavableConfig.ConfigVisit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Test.ConfigVisit
{
    [TestClass]
    public class ConfigurationVisitorTest
    {
        class ValueConfigurationVisitor : ConfigurationVisitor<ConfigurationKeyPart>
        {
            public ValueConfigurationVisitor(string[] parts) : base(parts)
            {
            }

            public new ConfigurationPartTypes GetPartType(string part)
            {
                return base.GetPartType(part);
            }

            protected override ConfigurationKeyPart MakeKeyPart(int partIndex)
            {
                return new ConfigurationKeyPart { PartIndex = partIndex };
            }
            public bool VisitOk { get; set; }
            public List<ConfigurationKeyPart> Visited { get; } = new List<ConfigurationKeyPart>();
            protected override bool VisitPart(ConfigurationKeyPart keyPart)
            {
                Visited.Add(keyPart);
                return VisitOk;
            }
        }
        [TestMethod]
        public void InitWithNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ValueConfigurationVisitor(null));
            Assert.ThrowsException<ArgumentException>(() => new ValueConfigurationVisitor(new string[] { null }));
            Assert.ThrowsException<ArgumentException>(() => new ValueConfigurationVisitor(new string[] { "" }));
            Assert.ThrowsException<ArgumentException>(() => new ValueConfigurationVisitor(new string[0]).GetPartType(null));
            Assert.ThrowsException<ArgumentException>(() => new ValueConfigurationVisitor(new string[0]).GetPartType(""));
        }
        [TestMethod]
        [DataRow("0")]
        [DataRow("1")]
        [DataRow("5")]
        [DataRow("100")]
        public void GetKeyPart_NumberMustArrayIndex(string part)
        {
            var set = new string[0];
            var ag = new ValueConfigurationVisitor(set);
            Assert.AreEqual(set, ag.Parts);
            var type = ag.GetPartType(part);
            Assert.AreEqual(ConfigurationPartTypes.ArrayIndex,type);
        }
        [TestMethod]
        [DataRow("你好")]
        [DataRow("hello")]
        [DataRow("안녕하세요")]
        [DataRow("こんにちは")]
        public void GetKeyPart_NumberMustNamed(string part)
        {
            var ag = new ValueConfigurationVisitor(new string[0]);
            var type = ag.GetPartType(part);
            Assert.AreEqual(ConfigurationPartTypes.Named, type);
        }
        [TestMethod]
        public void Visit_AllPartVisited()
        {
            var set = new string[]
            {
                "hello",
                "world",
                "I",
                "am",
                "fine"
            };
            var ag = new ValueConfigurationVisitor(set);
            ag.VisitOk = true;
            ag.VisitWrite();
            var notVisitPart = set.Except(ag.Parts).ToArray();
            if (notVisitPart.Length!=0)
            {
                Assert.Fail("The parts {0} does not visited",string.Join(",",notVisitPart));
            }
        }
    }
}
