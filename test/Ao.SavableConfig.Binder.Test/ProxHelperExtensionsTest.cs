using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Binder.Visitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class ProxHelperExtensionsTest
    {
        public class NullClass
        {
            public virtual int Age { get; set; }

            public virtual string Name { get; set; }
        }
        public class WithPathClass
        {
            public virtual int Age { get; set; }
            
            public virtual string Name { get; set; }
        }
        public class ComplexClass
        {
            [ConfigStepIn]
            public NullClass A { get; set; }

            [ConfigStepIn]
            public WithPathClass B { get; set; }

            [ConfigStepIn]
            public WithPathClass BX { get; set; }

            public virtual int? C { get; set; }

            public virtual string D { get; set; }

            public virtual float E { get; set; }
        }
        [TestMethod]
        public void GivenNullInvoke_MustThrowException()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var proxy = ProxyUtil.CreateProx();
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.EnsureCreateProx<object>(null, root, NullNameTransfer.Instance));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.EnsureCreateProx<object>(proxy, null, NullNameTransfer.Instance));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.EnsureCreateProx<object>(proxy, root, null));

            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.EnsureCreateProxWithAttribute<object>(proxy, null));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.EnsureCreateProxWithAttribute<object>(null, root));

            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(proxy, null));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(null, NullNameTransfer.Instance));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(proxy, NullNameTransfer.Instance, null));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(proxy, null, IdentityNamedCreator.Instance));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(null, NullNameTransfer.Instance, IdentityNamedCreator.Instance));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(proxy, NullNameTransfer.Instance, IdentityNamedCreator.Instance, null));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(proxy, NullNameTransfer.Instance, null, CompilePropertyVisitor.Instance));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(proxy, null, IdentityNamedCreator.Instance, CompilePropertyVisitor.Instance));
            Assert.ThrowsException<ArgumentNullException>(() => ProxHelperExtensions.CreateComplexProxy<object>(null, NullNameTransfer.Instance, IdentityNamedCreator.Instance, CompilePropertyVisitor.Instance));
        }
        [TestMethod]
        public void EnsureCreateProx_MustCreated()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var proxy = ProxyUtil.CreateProx();
            var val= proxy.EnsureCreateProx<NullClass>(root,IdentityMapNameTransfer.FromTypeAttributes(typeof(NullClass)));
            Assert.IsTrue(proxy.ProxMap.ContainsKey(typeof(NullClass)));
            Assert.IsNotNull(val);
        }
        [TestMethod]
        public void CreateComplaxClassProx_MustOk()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var proxy = ProxyUtil.CreateProx();
            var val = proxy.CreateComplexProxy<ComplexClass>();
            Assert.IsNotNull(val);
            Assert.AreEqual(proxy, val.ProxyHelper);
            var v = val.Build(root);
            Assert.IsNotNull(v);
            Assert.IsInstanceOfType(v, typeof(ComplexClass));
        }

        [TestMethod]
        public void ExistsEnsureCreateProx_WithIdentity_MustCreated()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var proxy = ProxyUtil.CreateProx();
            proxy.BuildProx(typeof(NullClass));
            var val = proxy.EnsureCreateProxWithAttribute<NullClass>(root);
            Assert.IsNotNull(val);
        }
        [TestMethod]
        public void ExistsEnsureCreateProx_MustCreated()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var proxy = ProxyUtil.CreateProx();
            proxy.BuildProx(typeof(NullClass));
            var val = proxy.EnsureCreateProx<NullClass>(root, IdentityMapNameTransfer.FromTypeAttributes(typeof(NullClass)));
            Assert.IsNotNull(val);
        }
    }
}
