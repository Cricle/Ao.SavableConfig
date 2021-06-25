using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ao.SavableConfig.Binder.Test.Compiling
{
    [TestClass]
    public class CompiledManagerTest
    {
        class ValueCompiledManager : CompiledManager<int, object>
        {
            protected override object Compile(int key)
            {
                return new object();
            }
        }
        [TestMethod]
        public void GetWhenNotCompiled_MustReturnDefault()
        {
            var mgr = new ValueCompiledManager();
            var val = mgr.GetCompiled(1);
            Assert.IsNull(val);
        }
        [TestMethod]
        public void GetWhenCompiled_MustReturnSingleInstance()
        {
            var mgr = new ValueCompiledManager();
            var a = mgr.EnsureGetCompiled(1);
            var b = mgr.GetCompiled(1);
            Assert.AreEqual(a, b);
            var c = mgr.EnsureGetCompiled(1);
            Assert.AreEqual(a, c);
        }
        [TestMethod]
        public void WhenCompiled_KeyOrCompiledHasDatas()
        {
            var mgr = new ValueCompiledManager();
            Assert.IsFalse(mgr.IsCompiled(1));
            var val = mgr.EnsureGetCompiled(1);
            Assert.AreEqual(1, mgr.Keys.Single());
            Assert.AreEqual(val, mgr.Complateds.Single());
            Assert.AreEqual(1, mgr.Count);
            Assert.IsTrue(mgr.IsCompiled(1));

            var enu = mgr.GetEnumerator();
            enu.MoveNext();
            Assert.AreEqual(1, enu.Current.Key);
            Assert.AreEqual(val, enu.Current.Value);

            var objenu = ((IEnumerable)mgr).GetEnumerator();

            objenu.MoveNext();
            var value = (KeyValuePair<int, object>)objenu.Current;
            Assert.AreEqual(1, value.Key);
            Assert.AreEqual(val, value.Value);

        }
    }
}
