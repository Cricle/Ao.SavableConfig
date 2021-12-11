// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Binder.Resources;
using System.Linq;

namespace Microsoft.Extensions.Configuration.FastBinder.Test
{
    [TestClass]
    public class ConfigurationBinderTests
    {
        public class ComplexOptions
        {
            public ComplexOptions()
            {
                Nested = new NestedOptions();
                Virtual = "complex";
            }

            public NestedOptions Nested { get; set; }
            public int Integer { get; set; }
            public bool Boolean { get; set; }
            public virtual string Virtual { get; set; }
            public object Object { get; set; }

            public string PrivateSetter { get; private set; }
            public string ProtectedSetter { get; protected set; }
            public string InternalSetter { get; internal set; }
            public static string StaticProperty { get; set; }

            private string PrivateProperty { get; set; }
            internal string InternalProperty { get; set; }
            protected string ProtectedProperty { get; set; }
#if USE_CONFIG_6_0

            [ConfigurationKeyName("Named_Property")]
            public string NamedProperty { get; set; }
#endif

            protected string ProtectedPrivateSet { get; private set; }

            private string PrivateReadOnly { get; }
            internal string InternalReadOnly { get; }
            protected string ProtectedReadOnly { get; }

            public string ReadOnly
            {
                get { return null; }
            }
        }

        public class NestedOptions
        {
            public int Integer { get; set; }
        }

        public class DerivedOptions : ComplexOptions
        {
            public override string Virtual
            {
                get
                {
                    return base.Virtual;
                }
                set
                {
                    base.Virtual = "Derived:" + value;
                }
            }
        }

        public class NullableOptions
        {
            public bool? MyNullableBool { get; set; }
            public int? MyNullableInt { get; set; }
            public DateTime? MyNullableDateTime { get; set; }
        }

        public class EnumOptions
        {
            public UriKind UriKind { get; set; }
        }

        public class GenericOptions<T>
        {
            public T Value { get; set; }
        }

        public class OptionsWithNesting
        {
            public NestedOptions Nested { get; set; }

            public class NestedOptions
            {
                public int Value { get; set; }
            }
        }

        public class ConfigurationInterfaceOptions
        {
            public IConfigurationSection Section { get; set; }
        }

        public class DerivedOptionsWithIConfigurationSection : DerivedOptions
        {
            public IConfigurationSection DerivedSection { get; set; }
        }

        public struct ValueTypeOptions
        {
            public int MyInt32 { get; set; }
            public string MyString { get; set; }
        }

        public class ByteArrayOptions
        {
            public byte[] MyByteArray { get; set; }
        }

        public class GetterOnlyOptions
        {
            public string MyString => throw new NotImplementedException();
        }

        [TestMethod]
        public void CanBindIConfigurationSection()
        {
            var dic = new Dictionary<string, string>
            {
                {"Section:Integer", "-2"},
                {"Section:Boolean", "TRUe"},
                {"Section:Nested:Integer", "11"},
                {"Section:Virtual", "Sup"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.FastGet<ConfigurationInterfaceOptions>();

            var childOptions = options.Section.FastGet<DerivedOptions>();

            Assert.IsTrue(childOptions.Boolean);
            Assert.AreEqual(-2, childOptions.Integer);
            Assert.AreEqual(11, childOptions.Nested.Integer);
            Assert.AreEqual("Derived:Sup", childOptions.Virtual);

            Assert.AreEqual("Section", options.Section.Key);
            Assert.AreEqual("Section", options.Section.Path);
            Assert.IsNull(options.Section.Value);
        }

        [TestMethod]
        public void CanBindWithKeyOverload()
        {
            var dic = new Dictionary<string, string>
            {
                {"Section:Integer", "-2"},
                {"Section:Boolean", "TRUe"},
                {"Section:Nested:Integer", "11"},
                {"Section:Virtual", "Sup"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = new DerivedOptions();
            config.FastBind("Section", options);

            Assert.IsTrue(options.Boolean);
            Assert.AreEqual(-2, options.Integer);
            Assert.AreEqual(11, options.Nested.Integer);
            Assert.AreEqual("Derived:Sup", options.Virtual);
        }

        [TestMethod]
        public void CanBindIConfigurationSectionWithDerivedOptionsSection()
        {
            var dic = new Dictionary<string, string>
            {
                {"Section:Integer", "-2"},
                {"Section:Boolean", "TRUe"},
                {"Section:Nested:Integer", "11"},
                {"Section:Virtual", "Sup"},
                {"Section:DerivedSection:Nested:Integer", "11"},
                {"Section:DerivedSection:Virtual", "Sup"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.FastGet<ConfigurationInterfaceOptions>();

            var childOptions = options.Section.FastGet<DerivedOptionsWithIConfigurationSection>();

            var childDerivedOptions = childOptions.DerivedSection.FastGet<DerivedOptions>();

            Assert.IsTrue(childOptions.Boolean);
            Assert.AreEqual(-2, childOptions.Integer);
            Assert.AreEqual(11, childOptions.Nested.Integer);
            Assert.AreEqual("Derived:Sup", childOptions.Virtual);
            Assert.AreEqual(11, childDerivedOptions.Nested.Integer);
            Assert.AreEqual("Derived:Sup", childDerivedOptions.Virtual);

            Assert.AreEqual("Section", options.Section.Key);
            Assert.AreEqual("Section", options.Section.Path);
            Assert.AreEqual("DerivedSection", childOptions.DerivedSection.Key);
            Assert.AreEqual("Section:DerivedSection", childOptions.DerivedSection.Path);
            Assert.IsNull(options.Section.Value);
        }
#if USE_CONFIG_6_0

        [TestMethod]
        public void CanBindConfigurationKeyNameAttributes()
        {
            var dic = new Dictionary<string, string>
            {
                {"Named_Property", "Yo"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.FastGet<ComplexOptions>();

            Assert.AreEqual("Yo", options.NamedProperty);
        }
#endif

        [TestMethod]
        public void EmptyStringIsNullable()
        {
            var dic = new Dictionary<string, string>
            {
                {"empty", ""},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            Assert.IsNull(config.FastGetValue<bool?>("empty"));
            Assert.IsNull(config.FastGetValue<int?>("empty"));
        }

        [TestMethod]
        public void GetScalarNullable()
        {
            var dic = new Dictionary<string, string>
            {
                {"Integer", "-2"},
                {"Boolean", "TRUe"},
                {"Nested:Integer", "11"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            Assert.IsTrue(config.FastGetValue<bool?>("Boolean").Value);
            Assert.AreEqual(-2, config.FastGetValue<int?>("Integer"));
            Assert.AreEqual(11, config.FastGetValue<int?>("Nested:Integer"));
        }

        [TestMethod]
        public void CanBindToObjectProperty()
        {
            var dic = new Dictionary<string, string>
            {
                {"Object", "whatever" }
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = new ComplexOptions();
            config.FastBind(options);

            Assert.AreEqual("whatever", options.Object);
        }

        [TestMethod]
        public void GetNullValue()
        {
            var dic = new Dictionary<string, string>
            {
                {"Integer", null},
                {"Boolean", null},
                {"Nested:Integer", null},
                {"Object", null }
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            Assert.IsFalse(config.FastGetValue<bool>("Boolean"));
            Assert.AreEqual(0, config.FastGetValue<int>("Integer"));
            Assert.AreEqual(0, config.FastGetValue<int>("Nested:Integer"));
            Assert.IsNull(config.FastGetValue<ComplexOptions>("Object"));
            Assert.IsFalse(config.GetSection("Boolean").FastGet<bool>());
            Assert.AreEqual(0, config.GetSection("Integer").FastGet<int>());
            Assert.AreEqual(0, config.GetSection("Nested:Integer").FastGet<int>());
            Assert.IsNull(config.GetSection("Object").FastGet<ComplexOptions>());
        }

        [TestMethod]
        public void ThrowsIfPropertyInConfigMissingInModel()
        {
            var dic = new Dictionary<string, string>
            {
                {"ThisDoesNotExistInTheModel", "42"},
                {"Integer", "-2"},
                {"Boolean", "TRUe"},
                {"Nested:Integer", "11"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var instance = new ComplexOptions();

            var ex = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastBind(instance, o => o.ErrorOnUnknownConfiguration = true));

            string expectedMessage = string.Format(Strings.Error_MissingConfig,
                nameof(FastBinderOptions.ErrorOnUnknownConfiguration), nameof(FastBinderOptions), typeof(ComplexOptions), "'ThisDoesNotExistInTheModel'");

            Assert.AreEqual(expectedMessage, ex.Message);
        }
        [TestMethod]
        public void ThrowsIfPropertyInConfigMissingInNestedModel()
        {
            var dic = new Dictionary<string, string>
            {
                {"Nested:ThisDoesNotExistInTheModel", "42"},
                {"Integer", "-2"},
                {"Boolean", "TRUe"},
                {"Nested:Integer", "11"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var instance = new ComplexOptions();

            string expectedMessage = string.Format(Strings.Error_MissingConfig,
                nameof(FastBinderOptions.ErrorOnUnknownConfiguration), nameof(FastBinderOptions), typeof(NestedOptions), "'ThisDoesNotExistInTheModel'");

            var ex = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastBind(instance, o => o.ErrorOnUnknownConfiguration = true));

            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [TestMethod]
        public void DoesNotExecuteGetterIfNoSetter()
        {
            var dic = new Dictionary<string, string>
            {
                {"MyString", "hello world"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var _ = config.FastGet<GetterOnlyOptions>();
        }

        [TestMethod]
        public void GetDefaultsWhenDataDoesNotExist()
        {
            var dic = new Dictionary<string, string>
            {
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            Assert.IsFalse(config.FastGetValue<bool>("Boolean"));
            Assert.AreEqual(0, config.FastGetValue<int>("Integer"));
            Assert.AreEqual(0, config.FastGetValue<int>("Nested:Integer"));
            Assert.IsNull(config.FastGetValue<ComplexOptions>("Object"));
            Assert.IsTrue(config.FastGetValue("Boolean", true));
            Assert.AreEqual(3, config.FastGetValue("Integer", 3));
            Assert.AreEqual(1, config.FastGetValue("Nested:Integer", 1));
            var foo = new ComplexOptions();
            Assert.AreSame(config.FastGetValue("Object", foo), foo);
        }

        [TestMethod]
        public void GetUri()
        {
            var dic = new Dictionary<string, string>
            {
                {"AnUri", "http://www.bing.com"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var uri = config.FastGetValue<Uri>("AnUri");

            Assert.AreEqual("http://www.bing.com", uri.OriginalString);
        }

        [TestMethod]
        [DataRow("2147483647", typeof(int))]
        [DataRow("4294967295", typeof(uint))]
        [DataRow("32767", typeof(short))]
        [DataRow("65535", typeof(ushort))]
        [DataRow("-9223372036854775808", typeof(long))]
        [DataRow("18446744073709551615", typeof(ulong))]
        [DataRow("trUE", typeof(bool))]
        [DataRow("255", typeof(byte))]
        [DataRow("127", typeof(sbyte))]
        [DataRow("\uffff", typeof(char))]
        [DataRow("79228162514264337593543950335", typeof(decimal))]
        [DataRow("1.79769e+308", typeof(double))]
        [DataRow("3.40282347E+38", typeof(float))]
        [DataRow("2015-12-24T07:34:42-5:00", typeof(DateTime))]
        [DataRow("12/24/2015 13:44:55 +4", typeof(DateTimeOffset))]
        [DataRow("99.22:22:22.1234567", typeof(TimeSpan))]
        [DataRow("http://www.bing.com", typeof(Uri))]
        // enum test
        [DataRow("Constructor", typeof(AttributeTargets))]
        [DataRow("CA761232-ED42-11CE-BACD-00AA0057B223", typeof(Guid))]
        public void CanReadAllSupportedTypes(string value, Type type)
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                {"Value", value}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var optionsType = typeof(GenericOptions<>).MakeGenericType(type);
            var options = Activator.CreateInstance(optionsType);
            var expectedValue = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);

            // act
            config.FastBind(options);
            var optionsValue = options.GetType().GetProperty("Value").GetValue(options);
            var getValueValue = config.FastGetValue(type, "Value");
            var getValue = config.GetSection("Value").FastGet(type);

            // assert
            Assert.AreEqual(expectedValue, optionsValue);
            Assert.AreEqual(expectedValue, getValue);
            Assert.AreEqual(expectedValue, getValueValue);
        }

        [TestMethod]
        [DataRow(typeof(int))]
        [DataRow(typeof(uint))]
        [DataRow(typeof(short))]
        [DataRow(typeof(ushort))]
        [DataRow(typeof(long))]
        [DataRow(typeof(ulong))]
        [DataRow(typeof(bool))]
        [DataRow(typeof(byte))]
        [DataRow(typeof(sbyte))]
        [DataRow(typeof(char))]
        [DataRow(typeof(decimal))]
        [DataRow(typeof(double))]
        [DataRow(typeof(float))]
        [DataRow(typeof(DateTime))]
        [DataRow(typeof(DateTimeOffset))]
        [DataRow(typeof(TimeSpan))]
        [DataRow(typeof(AttributeTargets))]
        [DataRow(typeof(Guid))]
        public void ConsistentExceptionOnFailedBinding(Type type)
        {
            // arrange
            const string IncorrectValue = "Invalid data";
            const string ConfigKey = "Value";
            var dic = new Dictionary<string, string>
            {
                {ConfigKey, IncorrectValue}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var optionsType = typeof(GenericOptions<>).MakeGenericType(type);
            var options = Activator.CreateInstance(optionsType);

            // act
            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastBind(options));

            var getValueException = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastGetValue(type, "Value"));

            var getException = Assert.ThrowsException<InvalidOperationException>(
                () => config.GetSection("Value").FastGet(type));

            // assert
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNotNull(getException.InnerException);
            Assert.AreEqual(
                string.Format(Strings.Error_FailedBinding, ConfigKey, type),
                exception.Message);
            Assert.AreEqual(
                string.Format(Strings.Error_FailedBinding, ConfigKey, type),
                getException.Message);
            Assert.AreEqual(
                string.Format(Strings.Error_FailedBinding, ConfigKey, type),
                getValueException.Message);
        }

        [TestMethod]
        public void ExceptionOnFailedBindingIncludesPath()
        {
            const string IncorrectValue = "Invalid data";
            const string ConfigKey = "Nested:Value";

            var dic = new Dictionary<string, string>
            {
                {ConfigKey, IncorrectValue}
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = new OptionsWithNesting();

            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastBind(options));

            Assert.AreEqual(string.Format(Strings.Error_FailedBinding, ConfigKey, typeof(int)),
                exception.Message);
        }

        [TestMethod]
        public void BinderIgnoresIndexerProperties()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var config = configurationBuilder.Build();
            config.FastBind(new List<string>());
        }

        [TestMethod]
        public void BindCanReadComplexProperties()
        {
            var dic = new Dictionary<string, string>
            {
                {"Integer", "-2"},
                {"Boolean", "TRUe"},
                {"Nested:Integer", "11"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var instance = new ComplexOptions();
            config.FastBind(instance);

            Assert.IsTrue(instance.Boolean);
            Assert.AreEqual(-2, instance.Integer);
            Assert.AreEqual(11, instance.Nested.Integer);
        }

        [TestMethod]
        public void GetCanReadComplexProperties()
        {
            var dic = new Dictionary<string, string>
            {
                {"Integer", "-2"},
                {"Boolean", "TRUe"},
                {"Nested:Integer", "11"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = new ComplexOptions();
            config.FastBind(options);

            Assert.IsTrue(options.Boolean);
            Assert.AreEqual(-2, options.Integer);
            Assert.AreEqual(11, options.Nested.Integer);
        }

        [TestMethod]
        public void BindCanReadInheritedProperties()
        {
            var dic = new Dictionary<string, string>
            {
                {"Integer", "-2"},
                {"Boolean", "TRUe"},
                {"Nested:Integer", "11"},
                {"Virtual", "Sup"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var instance = new DerivedOptions();
            config.FastBind(instance);

            Assert.IsTrue(instance.Boolean);
            Assert.AreEqual(-2, instance.Integer);
            Assert.AreEqual(11, instance.Nested.Integer);
            Assert.AreEqual("Derived:Sup", instance.Virtual);
        }

        [TestMethod]
        public void GetCanReadInheritedProperties()
        {
            var dic = new Dictionary<string, string>
            {
                {"Integer", "-2"},
                {"Boolean", "TRUe"},
                {"Nested:Integer", "11"},
                {"Virtual", "Sup"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = new DerivedOptions();
            config.FastBind(options);

            Assert.IsTrue(options.Boolean);
            Assert.AreEqual(-2, options.Integer);
            Assert.AreEqual(11, options.Nested.Integer);
            Assert.AreEqual("Derived:Sup", options.Virtual);
        }

        [TestMethod]
        public void GetCanReadStaticProperty()
        {
            var dic = new Dictionary<string, string>
            {
                {"StaticProperty", "stuff"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();
            var options = new ComplexOptions();
            config.FastBind(options);

            Assert.AreEqual("stuff", ComplexOptions.StaticProperty);
        }

        [TestMethod]
        public void BindCanReadStaticProperty()
        {
            var dic = new Dictionary<string, string>
            {
                {"StaticProperty", "other stuff"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var instance = new ComplexOptions();
            config.FastBind(instance);

            Assert.AreEqual("other stuff", ComplexOptions.StaticProperty);
        }

        [TestMethod]
        public void CanGetComplexOptionsWhichHasAlsoHasValue()
        {
            var dic = new Dictionary<string, string>
            {
                {"obj", "whut" },
                {"obj:Integer", "-2"},
                {"obj:Boolean", "TRUe"},
                {"obj:Nested:Integer", "11"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.GetSection("obj").FastGet<ComplexOptions>();
            Assert.IsNotNull(options);
            Assert.IsTrue(options.Boolean);
            Assert.AreEqual(-2, options.Integer);
            Assert.AreEqual(11, options.Nested.Integer);
        }

        [TestMethod]
        [DataRow("ReadOnly")]
        [DataRow("PrivateSetter")]
        [DataRow("ProtectedSetter")]
        [DataRow("InternalSetter")]
        [DataRow("InternalProperty")]
        [DataRow("PrivateProperty")]
        [DataRow("ProtectedProperty")]
        [DataRow("ProtectedPrivateSet")]
        public void GetIgnoresTests(string property)
        {
            var dic = new Dictionary<string, string>
            {
                {property, "stuff"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.FastGet<ComplexOptions>();
            Assert.IsNull(options.GetType().GetTypeInfo().GetDeclaredProperty(property).GetValue(options));
        }

        [TestMethod]
        [DataRow("PrivateSetter")]
        [DataRow("ProtectedSetter")]
        [DataRow("InternalSetter")]
        [DataRow("InternalProperty")]
        [DataRow("PrivateProperty")]
        [DataRow("ProtectedProperty")]
        [DataRow("ProtectedPrivateSet")]
        public void GetCanSetNonPublicWhenSet(string property)
        {
            var dic = new Dictionary<string, string>
            {
                {property, "stuff"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.FastGet<ComplexOptions>(o => o.BindNonPublicProperties = true);
            Assert.AreEqual("stuff", options.GetType().GetTypeInfo().GetDeclaredProperty(property).GetValue(options));
        }

        [TestMethod]
        [DataRow("InternalReadOnly")]
        [DataRow("PrivateReadOnly")]
        [DataRow("ProtectedReadOnly")]
        public void NonPublicModeGetStillIgnoresReadonly(string property)
        {
            var dic = new Dictionary<string, string>
            {
                {property, "stuff"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.FastGet<ComplexOptions>(o => o.BindNonPublicProperties = true);
            Assert.IsNull(options.GetType().GetTypeInfo().GetDeclaredProperty(property).GetValue(options));
        }

        [TestMethod]
        [DataRow("ReadOnly")]
        [DataRow("PrivateSetter")]
        [DataRow("ProtectedSetter")]
        [DataRow("InternalSetter")]
        [DataRow("InternalProperty")]
        [DataRow("PrivateProperty")]
        [DataRow("ProtectedProperty")]
        [DataRow("ProtectedPrivateSet")]
        public void BindIgnoresTests(string property)
        {
            var dic = new Dictionary<string, string>
            {
                {property, "stuff"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = new ComplexOptions();
            config.FastBind(options);

            Assert.IsNull(options.GetType().GetTypeInfo().GetDeclaredProperty(property).GetValue(options));
        }

        [TestMethod]
        [DataRow("PrivateSetter")]
        [DataRow("ProtectedSetter")]
        [DataRow("InternalSetter")]
        [DataRow("InternalProperty")]
        [DataRow("PrivateProperty")]
        [DataRow("ProtectedProperty")]
        [DataRow("ProtectedPrivateSet")]
        public void BindCanSetNonPublicWhenSet(string property)
        {
            var dic = new Dictionary<string, string>
            {
                {property, "stuff"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = new ComplexOptions();
            config.FastBind(options, o => o.BindNonPublicProperties = true);
            Assert.AreEqual("stuff", options.GetType().GetTypeInfo().GetDeclaredProperty(property).GetValue(options));
        }

        [TestMethod]
        [DataRow("InternalReadOnly")]
        [DataRow("PrivateReadOnly")]
        [DataRow("ProtectedReadOnly")]
        public void NonPublicModeBindStillIgnoresReadonly(string property)
        {
            var dic = new Dictionary<string, string>
            {
                {property, "stuff"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = new ComplexOptions();
            config.FastBind(options, o => o.BindNonPublicProperties = true);
            Assert.IsNull(options.GetType().GetTypeInfo().GetDeclaredProperty(property).GetValue(options));
        }

        [TestMethod]
        public void ExceptionWhenTryingToBindToInterface()
        {
            var input = new Dictionary<string, string>
            {
                {"ISomeInterfaceProperty:Subkey", "x"}
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(input);
            var config = configurationBuilder.Build();

            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastBind(new TestOptions()));
            Assert.AreEqual(
                string.Format(Strings.Error_CannotActivateAbstractOrInterface, typeof(ISomeInterface)),
                exception.Message);
        }

        [TestMethod]
        public void ExceptionWhenTryingToBindClassWithoutParameterlessConstructor()
        {
            var input = new Dictionary<string, string>
            {
                {"ClassWithoutPublicConstructorProperty:Subkey", "x"}
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(input);
            var config = configurationBuilder.Build();

            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastBind(new TestOptions()));
            Assert.AreEqual(
                string.Format(Strings.Error_MissingParameterlessConstructor, typeof(ClassWithoutPublicConstructor)),
                exception.Message);
        }

        [TestMethod]
        public void ExceptionWhenTryingToBindToTypeThrowsWhenActivated()
        {
            var input = new Dictionary<string, string>
            {
                {"ThrowsWhenActivatedProperty:subkey", "x"}
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(input);
            var config = configurationBuilder.Build();

            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastBind(new TestOptions()));
            Assert.IsNotNull(exception.InnerException);
            Assert.AreEqual(
                string.Format(Strings.Error_FailedToActivate, typeof(ThrowsWhenActivated)),
                exception.Message);
        }

        [TestMethod]
        public void ExceptionIncludesKeyOfFailedBinding()
        {
            var input = new Dictionary<string, string>
            {
                {"NestedOptionsProperty:NestedOptions2Property:ISomeInterfaceProperty:subkey", "x"}
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(input);
            var config = configurationBuilder.Build();

            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastBind(new TestOptions()));
            Assert.AreEqual(
                string.Format(Strings.Error_CannotActivateAbstractOrInterface, typeof(ISomeInterface)),
                exception.Message);
        }

        [TestMethod]
        public void CanBindValueTypeOptions()
        {
            var dic = new Dictionary<string, string>
            {
                {"MyInt32", "42"},
                {"MyString", "hello world"},
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.FastGet<ValueTypeOptions>();
            Assert.AreEqual(42, options.MyInt32);
            Assert.AreEqual("hello world", options.MyString);
        }

        [TestMethod]
        public void CanBindByteArray()
        {
            var bytes = new byte[] { 1, 2, 3, 4 };
            var dic = new Dictionary<string, string>
            {
                { "MyByteArray", Convert.ToBase64String(bytes) }
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.FastGet<ByteArrayOptions>();
            Assert.IsTrue(bytes.SequenceEqual(options.MyByteArray));
        }

        [TestMethod]
        public void CanBindByteArrayWhenValueIsNull()
        {
            var dic = new Dictionary<string, string>
            {
                { "MyByteArray", null }
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var options = config.FastGet<ByteArrayOptions>();
            Assert.IsNull(options.MyByteArray);
        }

        [TestMethod]
        public void ExceptionWhenTryingToBindToByteArray()
        {
            var dic = new Dictionary<string, string>
            {
                { "MyByteArray", "(not a valid base64 string)" }
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(dic);
            var config = configurationBuilder.Build();

            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => config.FastGet<ByteArrayOptions>());
            Assert.AreEqual(
                string.Format(Strings.Error_FailedBinding, "MyByteArray", typeof(byte[])),
                exception.Message);
        }

        private interface ISomeInterface
        {
        }

        private class ClassWithoutPublicConstructor
        {
            private ClassWithoutPublicConstructor()
            {
            }
        }

        private class ThrowsWhenActivated
        {
            public ThrowsWhenActivated()
            {
                throw new Exception();
            }
        }

        private class NestedOptions1
        {
            public NestedOptions2 NestedOptions2Property { get; set; }
        }

        private class NestedOptions2
        {
            public ISomeInterface ISomeInterfaceProperty { get; set; }
        }

        private class TestOptions
        {
            public ISomeInterface ISomeInterfaceProperty { get; set; }

            public ClassWithoutPublicConstructor ClassWithoutPublicConstructorProperty { get; set; }

            public int IntProperty { get; set; }

            public ThrowsWhenActivated ThrowsWhenActivatedProperty { get; set; }

            public NestedOptions1 NestedOptionsProperty { get; set; }
        }
    }
}