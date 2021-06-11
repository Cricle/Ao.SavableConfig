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
    public class CompilePropertyVisitorTest
    {
        class ValueBox
        {
            public int Value { get; set; }
        }
        [TestMethod]
        public void GetOrSetPropery_MustEqualOrSetInstanceValue()
        {
            var visitor = CompilePropertyVisitor.Instance;

            var valueProp = typeof(ValueBox).GetProperty(nameof(ValueBox.Value));

            var obj = new ValueBox();
            visitor.SetValue(obj, 123, valueProp);
            Assert.AreEqual(123, obj.Value);

            var val = (int)visitor.GetValue(obj, valueProp);
            Assert.AreEqual(123, val);
        }
    }
}
