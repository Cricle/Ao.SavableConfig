using Ao.SavableConfig.Binder.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class BinderConfigurationExtensionsTest
    {
        public class ComplexClass
        {
            [ConfigStepIn]
            public NullClass Value { get; set; }
        }
        public class NullClass
        {
            public virtual int? Age { get; set; }

            public virtual string Name { get; set; }
        }
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            var root = ConfigHelper.CreateEmptyRoot();

            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.AutoCreateProxy<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.AutoCreateProxy(root,null));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.CreateComplexProxy<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.CreateComplexProxy(root,null));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.CreateProxy<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.CreateProxy<object>(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.CreateProxy(root, null));

            Assert.ThrowsException<ArgumentException>(() => BinderConfigurationExtensions.AutoCreateProxy<object>(root, (string)null));
            Assert.ThrowsException<ArgumentException>(() => BinderConfigurationExtensions.AutoCreateProxy<object>(root, null, NullNameTransfer.Instance));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.CreateProxy<object>(null, NullNameTransfer.Instance));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.CreateProxy(root,null, NullNameTransfer.Instance));
        }
        [TestMethod]
        public void CreateComplex_MustOk()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var c = BinderConfigurationExtensions.CreateComplexProxy<ComplexClass>(root);
            Assert.IsNotNull(c);
        }
        [TestMethod]
        public void CreateProxy_MustOk()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var c = BinderConfigurationExtensions.CreateProxy<NullClass>(root);
            Assert.IsNotNull(c);
            c = BinderConfigurationExtensions.CreateProxy<NullClass>(root, NullNameTransfer.Instance);
            Assert.IsNotNull(c);
        }
        [TestMethod]
        public void AutoCreateProxy_MustOk()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var c = BinderConfigurationExtensions.AutoCreateProxy<NullClass>(root);
            Assert.IsNotNull(c);
            c = BinderConfigurationExtensions.AutoCreateProxy<NullClass>(root, "T");
            Assert.IsNotNull(c);
            c = BinderConfigurationExtensions.AutoCreateProxy<NullClass>(root, "T", NullNameTransfer.Instance);
            Assert.IsNotNull(c);
            var d = BinderConfigurationExtensions.AutoCreateProxy<ComplexClass>(root);
            Assert.IsNotNull(d);
            d = BinderConfigurationExtensions.AutoCreateProxy<ComplexClass>(root, "T");
            Assert.IsNotNull(d);
            d = BinderConfigurationExtensions.AutoCreateProxy<ComplexClass>(root, NullNameTransfer.Instance);
            Assert.IsNotNull(d);
        }
    }
}
