using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ao.SavableConfig.Binder.Test.Compiling
{
    [TestClass]
    public class ConstructCompiledTest
    {
        class ValueBox
        {
            public ValueBox() { }
            public ValueBox(int a) { }
            public ValueBox(object a) { }
            public ValueBox(int a, object b, double c) { }
        }
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConstructCompiled(null));
        }
        [TestMethod]
        public void GivenEqualOrNotValue_MustEqualOrNot()
        {
            var constA = typeof(ValueBox).GetConstructor(Type.EmptyTypes);
            var constB = typeof(ValueBox).GetConstructor(new Type[] { typeof(int) });

            var a = new ConstructCompiled(constA);
            var b = new ConstructCompiled(constA);
            var c = new ConstructCompiled(constB);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsFalse(a.Equals(c));
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        }
        private void Run(Type[] types, object[] values)
        {
            var construct = typeof(ValueBox).GetConstructor(types);
            var c = new ConstructCompiled(construct);
            Assert.AreEqual(construct, c.Constructor);
            var val = c.Creator(values);
            Assert.IsNotNull(val, "arg count" + values.Length);
        }
        [TestMethod]
        public void BuildConstruct_Create_MustInstanceValue()
        {
            Run(new Type[0], new object[0]);
            Run(new Type[] { typeof(int) }, new object[] { 1 });
            Run(new Type[] { typeof(object) }, new object[] { 123 });
            Run(new Type[] { typeof(int), typeof(object), typeof(double) }, new object[] { 1, 2, 3.4 });
        }
    }
}
