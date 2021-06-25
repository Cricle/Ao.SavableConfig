using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
            Assert.ThrowsException<ArgumentNullException>(() => new PropertyCompiled(typeof(ValueBox), null));
            Assert.ThrowsException<ArgumentNullException>(() => new PropertyCompiled(null, typeof(ValueBox).GetProperty(nameof(ValueBox.Value))));
        }
        [TestMethod]
        public void GetOrSetValue_ValueMustBeVisitOrSet()
        {
            var prop = typeof(ValueBox).GetProperty(nameof(ValueBox.Value));

            var pc = new PropertyCompiled(typeof(ValueBox), prop);
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

            var pc = new PropertyCompiled(typeof(ValueBox), prop);
            Assert.IsNull(pc.Setter);
            Assert.IsNotNull(pc.Getter);
        }
        [TestMethod]
        public void GivenEqualOrNotComparer_MustReturnEqualOrNot()
        {
            var prop = typeof(ValueBox).GetProperty(nameof(ValueBox.ReadOnly));
            var prop2 = typeof(ValueBox).GetProperty(nameof(ValueBox.Value));

            var a = new PropertyCompiled(typeof(ValueBox), prop);
            var b = new PropertyCompiled(typeof(ValueBox), prop);
            var c = new PropertyCompiled(typeof(ValueBox), prop2);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsFalse(b.Equals(c));
            Assert.IsFalse(b.Equals((object)null));
            Assert.IsFalse(b.Equals(null));

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        }
    }
}
