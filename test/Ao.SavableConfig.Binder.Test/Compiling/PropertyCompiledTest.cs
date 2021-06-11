using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test.Compiling
{
    [TestClass]
    public class PropertyCompiledTest
    {
        class ValueBox
        {
            public int ReadOnly { get; }

            public int Value { get; set; }
        }
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PropertyCompiled(null));
        }
        [TestMethod]
        public void GetOrSetValue_ValueMustBeVisitOrSet()
        {
            var prop = typeof(ValueBox).GetProperty(nameof(ValueBox.Value));

            var pc = new PropertyCompiled(prop);
            var inst = new ValueBox();

            pc.Setter(inst, 123);
            Assert.AreEqual(123, inst.Value);

            var val = (int)pc.Getter(inst);
            Assert.AreEqual(123, val);
        }
        [TestMethod]
        public void BuildReadonlyProperty_SetterMustNull()
        {
            var prop = typeof(ValueBox).GetProperty(nameof(ValueBox.ReadOnly));

            var pc = new PropertyCompiled(prop);
            Assert.IsNull(pc.Setter);
            Assert.IsNotNull(pc.Getter);
        }
        [TestMethod]
        public void GivenEqualOrNotComparer_MustReturnEqualOrNot()
        {
            var prop = typeof(ValueBox).GetProperty(nameof(ValueBox.ReadOnly));
            var prop2 = typeof(ValueBox).GetProperty(nameof(ValueBox.Value));

            var a = new PropertyCompiled(prop);
            var b = new PropertyCompiled(prop);
            var c = new PropertyCompiled(prop2);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsFalse(b.Equals(c));
            Assert.IsFalse(b.Equals((object)null));
            Assert.IsFalse(b.Equals((PropertyCompiled)null));

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        }
    }
}
