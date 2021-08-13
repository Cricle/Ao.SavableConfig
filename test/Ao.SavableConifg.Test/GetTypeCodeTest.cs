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
    public class TypeHelperTest
    {
        [TestMethod]
        [DataRow(null, ConfigurationTypes.Null)]
        [DataRow("", ConfigurationTypes.String)]
        [DataRow("false", ConfigurationTypes.Boolean)]
        [DataRow("true", ConfigurationTypes.Boolean)]
        [DataRow("123", ConfigurationTypes.Number)]
        [DataRow("-123", ConfigurationTypes.Number)]
        [DataRow("123.1231231231", ConfigurationTypes.Single)]
        public void GetTypeCode_Returns(string val, ConfigurationTypes type)
        {
            var res = TypeHelper.GetTypeCode(val);

            Assert.AreEqual(type, res);
        }
        [TestMethod]
        public void GetTypeCodeByNoneType_Returns()
        {
            var res = TypeHelper.GetTypeCode("hello");

            Assert.AreEqual(ConfigurationTypes.String, res);
        }
    }
}
