﻿using Ao.SavableConfig.Binder.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.AutoCreateProxy<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.CreateComplexProxy<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => BinderConfigurationExtensions.CreateProxy<object>(null));

            var root = ConfigHelper.CreateEmptyRoot();
            Assert.ThrowsException<ArgumentException>(() => BinderConfigurationExtensions.AutoCreateProxy<object>(root,(string)null));
        }
        [TestMethod]
        public void CreateComplex_MustOk()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var c=BinderConfigurationExtensions.CreateComplexProxy<ComplexClass>(root);
            Assert.IsNotNull(c);
        }
        [TestMethod]
        public void CreateProxy_MustOk()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var c = BinderConfigurationExtensions.CreateProxy<NullClass>(root);
            Assert.IsNotNull(c);
            c = BinderConfigurationExtensions.CreateProxy<NullClass>(root,new NullNameTransfer());
            Assert.IsNotNull(c);
        }
        [TestMethod]
        public void AutoCreateProxy_MustOk()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var c = BinderConfigurationExtensions.AutoCreateProxy<NullClass>(root);
            Assert.IsNotNull(c);
            c = BinderConfigurationExtensions.AutoCreateProxy<NullClass>(root,"T");
            Assert.IsNotNull(c);
            c = BinderConfigurationExtensions.AutoCreateProxy<NullClass>(root, "T",new NullNameTransfer());
            Assert.IsNotNull(c);
            var d = BinderConfigurationExtensions.AutoCreateProxy<ComplexClass>(root);
            Assert.IsNotNull(d);
            d = BinderConfigurationExtensions.AutoCreateProxy<ComplexClass>(root,"T");
            Assert.IsNotNull(d);
            d = BinderConfigurationExtensions.AutoCreateProxy<ComplexClass>(root,new NullNameTransfer());
            Assert.IsNotNull(d);
        }
    }
}