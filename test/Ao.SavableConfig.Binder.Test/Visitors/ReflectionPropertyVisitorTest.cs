using Ao.SavableConfig.Binder.Visitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test.Visitors
{
    [TestClass]
    public class ReflectionPropertyVisitorTest
    {
        class ValueBox
        {
            public string Value { get; set; }
        }
        [TestMethod]
        public void CallGetOrSetProperty_MustVisitProperty()
        {
            var valueProp = typeof(ValueBox).GetProperty(nameof(ValueBox.Value));

            var pv = ReflectionPropertyVisitor.Instance;

            var val = new ValueBox();
            pv.SetValue(val, "123", valueProp);
            Assert.AreEqual("123", val.Value);
            var v = (string)pv.GetValue(val, valueProp);

            Assert.AreEqual("123", v);
        }
    }
}
