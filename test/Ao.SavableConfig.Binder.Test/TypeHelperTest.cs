using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class TypeHelperTest
    {
        [TestMethod]
        [DataRow(typeof(int))]
        [DataRow(typeof(long))]
        [DataRow(typeof(int?))]
        [DataRow(typeof(long?))]
        [DataRow(typeof(string))]
        [DataRow(typeof(decimal))]
        [DataRow(typeof(decimal?))]
        [DataRow(typeof(double))]
        [DataRow(typeof(double?))]
        public void GivenBaseType_MustReturnTrue(Type type)
        {
            Assert.IsTrue(TypeHelper.IsBaseType(type));
        }
        [TestMethod]
        [DataRow(typeof(object))]
        [DataRow(typeof(Assert))]
        [DataRow(typeof(void))]
        public void GivenNoBaseType_MustReturnFalse(Type type)
        {
            Assert.IsFalse(TypeHelper.IsBaseType(type));
        }
        [TestMethod]
        [DataRow("1", typeof(int), 1)]
        [DataRow("1.123", typeof(float), 1.123f)]
        [DataRow("1.123", typeof(double), 1.123d)]
        [DataRow("10000", typeof(long), 10000L)]
        [DataRow("10000", typeof(string), "10000")]
        [DataRow("1", typeof(int?), 1)]
        [DataRow("1.123", typeof(float?), 1.123f)]
        [DataRow("1.123", typeof(double?), 1.123d)]
        [DataRow("10000", typeof(long?), 10000L)]
        public void ChangeType_MustBeChanged(string input, Type type, object value)
        {
            var t = type;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                t = Nullable.GetUnderlyingType(t);
            }
            var val = TypeHelper.TryChangeType(input, type, out var ex, out var res);
            Assert.IsTrue(val);
            Assert.IsNull(ex);
            Assert.IsInstanceOfType(res, t);
            Assert.AreEqual(value, res);
        }
        [TestMethod]
        [DataRow("a", typeof(int))]
        [DataRow("ab", typeof(float))]
        [DataRow("abc", typeof(double))]
        [DataRow("abcd", typeof(long))]
        [DataRow("abcde", typeof(int?))]
        [DataRow("abcdef", typeof(float?))]
        [DataRow("abcdefg", typeof(double?))]
        [DataRow("abcdefgh", typeof(long?))]
        public void ChangeUnconvetedType_MustFail(string input, Type type)
        {
            var t = type;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                t = Nullable.GetUnderlyingType(t);
            }
            var val = TypeHelper.TryChangeType(input, type, out var ex, out var res);
            Assert.IsFalse(val);
            Assert.IsNotNull(ex);
            if (t != type)
            {
                Assert.IsNull(res);
            }
        }
        [TestMethod]
        public void FalseCase()
        {
            var val = TypeHelper.TryChangeType("123.123", typeof(int), out var ex, out _);
            Assert.IsFalse(val);
            Assert.IsNotNull(ex);
        }
        [TestMethod]
        public void FalseCaseEnum()
        {
            var val = TypeHelper.TryChangeType("123.123", typeof(ConsoleKey), out var ex, out _);
            Assert.IsFalse(val);
            Assert.IsNotNull(ex);
        }
    }
}
