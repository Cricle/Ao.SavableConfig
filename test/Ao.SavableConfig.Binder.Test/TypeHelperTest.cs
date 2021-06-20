using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
