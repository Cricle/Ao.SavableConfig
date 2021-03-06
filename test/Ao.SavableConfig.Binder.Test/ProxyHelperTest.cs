using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ao.SavableConfig.Binder.Test
{
    internal static class ProxyUtil
    {
        private static int index;
        public static ProxyHelper CreateProx()
        {
            var ass = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("a" + index++), AssemblyBuilderAccess.RunAndCollect);
            var md = ass.DefineDynamicModule("dy");
            return new ProxyHelper(ass, md);
        }
    }
    [TestClass]
    public class ProxyHelperTest
    {
        public struct NullStruct
        {

        }
        public class Setting1
        {
            public virtual int Age { get; set; }

            public virtual string Name { get; set; }

            public virtual int? CanBuild { get; set; }

            public virtual NullStruct? Any { get; set; }
        }
        [TestMethod]
        public void Create_MustUsingDefault()
        {
            var val = ObjectNamedCreator.Create(typeof(Setting1));
            Assert.AreEqual(typeof(Setting1), val.Type);
        }
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            var ass = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("NullInit"), AssemblyBuilderAccess.RunAndCollect);
            var md = ass.DefineDynamicModule("dy");
            var root = ConfigHelper.CreateEmptyRoot();
            var transfer = NullNameTransfer.Instance;
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyHelper(null, md));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyHelper(ass, null));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyHelper(ass, md).BuildProx(null));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyHelper(ass, md).CreateProxy(null, root, transfer));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyHelper(ass, md).CreateProxy(typeof(object), null, transfer));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyHelper(ass, md).CreateProxy(typeof(object), root, null));
        }
        [TestMethod]
        public void CreateWhenNotBuild_MustReturnNull()
        {
            var prox = ProxyUtil.CreateProx();
            var root = ConfigHelper.CreateEmptyRoot();
            var val = prox.CreateProxy(typeof(Setting1), root, NullNameTransfer.Instance);
            Assert.IsNull(val);
        }
        [TestMethod]
        public void BuildTwice_MustReturnFail()
        {
            var prox = ProxyUtil.CreateProx();
            _ = ConfigHelper.CreateEmptyRoot();
            Assert.IsNull(prox.GetProxyType(typeof(Setting1)));
            var val = prox.BuildProx(typeof(Setting1));
            Assert.IsTrue(val);
            Assert.IsNotNull(prox.GetProxyType(typeof(Setting1)));
            val = prox.BuildProx(typeof(Setting1));
            Assert.IsFalse(val);
        }
        [TestMethod]
        public void InitWithArgs_PropertyValueMustEqualInput()
        {
            var ass = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("WithArgs"), AssemblyBuilderAccess.RunAndCollect);
            var md = ass.DefineDynamicModule("dy");
            var prox = new ProxyHelper(ass, md);
            Assert.AreEqual(ass, prox.DynamicAssembly);
            Assert.AreEqual(md, prox.DynamicModule);
        }
        [TestMethod]
        public void GetTwiceDefault_MustEqual()
        {
            var a = ProxyHelper.Default;
            var b = ProxyHelper.Default;
            Assert.AreEqual(a, b);
        }
        [TestMethod]
        public void BuildType_MustOk()
        {
            var proxType = ProxyUtil.CreateProx();
            var res = proxType.BuildProx(typeof(Setting1));
            Assert.IsTrue(res);
            Assert.AreEqual(1, proxType.ProxMap.Count);
            Assert.AreEqual(typeof(Setting1), proxType.ProxMap.Keys.First());
            var newType = proxType.GetProxyType(typeof(Setting1));
            Assert.IsTrue(newType.BaseType == typeof(Setting1));
        }
        [TestMethod]
        public void CreateProxType_MustOk()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var proxType = ProxyUtil.CreateProx();
            var res = proxType.BuildProx(typeof(Setting1));
            Assert.IsTrue(res);
            var newType = proxType.CreateProxy(typeof(Setting1), root, IdentityMapNameTransfer.FromTypeAttributes(typeof(Setting1)));
            Assert.IsNotNull(newType);
            Assert.IsInstanceOfType(newType, typeof(Setting1));
        }
    }
}
