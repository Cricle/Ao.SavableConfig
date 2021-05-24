// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Ao.SavableConfig;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Extensions.Configuration.Test
{
    /// <summary>
    /// From https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Configuration/tests/ConfigurationTest.cs
    /// </summary>
    [TestClass]
    public class ConfigurationTest
    {
        public void LoadAndCombineKeyValuePairsFromDifferentConfigurationProviders()
        {
            // Arrange
            var dic1 = new Dictionary<string, string>()
            {
                {"Mem1:KeyInMem1", "ValueInMem1"}
            };
            var dic2 = new Dictionary<string, string>()
            {
                {"Mem2:KeyInMem2", "ValueInMem2"}
            };
            var dic3 = new Dictionary<string, string>()
            {
                {"Mem3:KeyInMem3", "ValueInMem3"}
            };
            var memConfigSrc1 = new MemoryConfigurationSource { InitialData = dic1 };
            var memConfigSrc2 = new MemoryConfigurationSource { InitialData = dic2 };
            var memConfigSrc3 = new MemoryConfigurationSource { InitialData = dic3 };

            var configurationBuilder = new SavableConfiurationBuilder();

            // Act
            configurationBuilder.Add(memConfigSrc1);
            configurationBuilder.Add(memConfigSrc2);
            configurationBuilder.Add(memConfigSrc3);

            var config = configurationBuilder.Build();

            var memVal1 = config["mem1:keyinmem1"];
            var memVal2 = config["Mem2:KeyInMem2"];
            var memVal3 = config["MEM3:KEYINMEM3"];

            // Assert
            Assert.IsTrue(configurationBuilder.Sources.Any(x => memConfigSrc1 == x));
            Assert.IsTrue(configurationBuilder.Sources.Any(x => memConfigSrc2 == x));
            Assert.IsTrue(configurationBuilder.Sources.Any(x => memConfigSrc3 == x));

            Assert.AreEqual("ValueInMem1", memVal1);
            Assert.AreEqual("ValueInMem2", memVal2);
            Assert.AreEqual("ValueInMem3", memVal3);

            Assert.AreEqual("ValueInMem1", config["mem1:keyinmem1"]);
            Assert.AreEqual("ValueInMem2", config["Mem2:KeyInMem2"]);
            Assert.AreEqual("ValueInMem3", config["MEM3:KEYINMEM3"]);
            Assert.IsNull(config["NotExist"]);
        }
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void AsEnumerateFlattensIntoDictionaryTest(bool removePath)
        {
            // Arrange
            var dic1 = new Dictionary<string, string>()
            {
                {"Mem1", "Value1"},
                {"Mem1:", "NoKeyValue1"},
                {"Mem1:KeyInMem1", "ValueInMem1"},
                {"Mem1:KeyInMem1:Deep1", "ValueDeep1"}
            };
            var dic2 = new Dictionary<string, string>()
            {
                {"Mem2", "Value2"},
                {"Mem2:", "NoKeyValue2"},
                {"Mem2:KeyInMem2", "ValueInMem2"},
                {"Mem2:KeyInMem2:Deep2", "ValueDeep2"}
            };
            var dic3 = new Dictionary<string, string>()
            {
                {"Mem3", "Value3"},
                {"Mem3:", "NoKeyValue3"},
                {"Mem3:KeyInMem3", "ValueInMem3"},
                {"Mem3:KeyInMem3:Deep3", "ValueDeep3"}
            };
            var memConfigSrc1 = new MemoryConfigurationSource { InitialData = dic1 };
            var memConfigSrc2 = new MemoryConfigurationSource { InitialData = dic2 };
            var memConfigSrc3 = new MemoryConfigurationSource { InitialData = dic3 };

            var configurationBuilder = new SavableConfiurationBuilder();

            // Act
            configurationBuilder.Add(memConfigSrc1);
            configurationBuilder.Add(memConfigSrc2);
            configurationBuilder.Add(memConfigSrc3);
            var config = configurationBuilder.Build();
            var dict = config.AsEnumerable(makePathsRelative: removePath).ToDictionary(k => k.Key, v => v.Value);

            // Assert
            Assert.AreEqual("Value1", dict["Mem1"]);
            Assert.AreEqual("NoKeyValue1", dict["Mem1:"]);
            Assert.AreEqual("ValueDeep1", dict["Mem1:KeyInMem1:Deep1"]);
            Assert.AreEqual("ValueInMem2", dict["Mem2:KeyInMem2"]);
            Assert.AreEqual("Value2", dict["Mem2"]);
            Assert.AreEqual("NoKeyValue2", dict["Mem2:"]);
            Assert.AreEqual("ValueDeep2", dict["Mem2:KeyInMem2:Deep2"]);
            Assert.AreEqual("Value3", dict["Mem3"]);
            Assert.AreEqual("NoKeyValue3", dict["Mem3:"]);
            Assert.AreEqual("ValueInMem3", dict["Mem3:KeyInMem3"]);
            Assert.AreEqual("ValueDeep3", dict["Mem3:KeyInMem3:Deep3"]);
        }

        [TestMethod]
        public void AsEnumerateStripsKeyFromChildren()
        {
            // Arrange
            var dic1 = new Dictionary<string, string>()
            {
                {"Mem1", "Value1"},
                {"Mem1:", "NoKeyValue1"},
                {"Mem1:KeyInMem1", "ValueInMem1"},
                {"Mem1:KeyInMem1:Deep1", "ValueDeep1"}
            };
            var dic2 = new Dictionary<string, string>()
            {
                {"Mem2", "Value2"},
                {"Mem2:", "NoKeyValue2"},
                {"Mem2:KeyInMem2", "ValueInMem2"},
                {"Mem2:KeyInMem2:Deep2", "ValueDeep2"}
            };
            var dic3 = new Dictionary<string, string>()
            {
                {"Mem3", "Value3"},
                {"Mem3:", "NoKeyValue3"},
                {"Mem3:KeyInMem3", "ValueInMem3"},
                {"Mem3:KeyInMem4", "ValueInMem4"},
                {"Mem3:KeyInMem3:Deep3", "ValueDeep3"},
                {"Mem3:KeyInMem3:Deep4", "ValueDeep4"}
            };
            var memConfigSrc1 = new MemoryConfigurationSource { InitialData = dic1 };
            var memConfigSrc2 = new MemoryConfigurationSource { InitialData = dic2 };
            var memConfigSrc3 = new MemoryConfigurationSource { InitialData = dic3 };

            var configurationBuilder = new SavableConfiurationBuilder();

            // Act
            configurationBuilder.Add(memConfigSrc1);
            configurationBuilder.Add(memConfigSrc2);
            configurationBuilder.Add(memConfigSrc3);

            var config = configurationBuilder.Build();

            var dict = config.GetSection("Mem1").AsEnumerable(makePathsRelative: true).ToDictionary(k => k.Key, v => v.Value);
            Assert.AreEqual(3, dict.Count);
            Assert.AreEqual("NoKeyValue1", dict[""]);
            Assert.AreEqual("ValueInMem1", dict["KeyInMem1"]);
            Assert.AreEqual("ValueDeep1", dict["KeyInMem1:Deep1"]);

            var dict2 = config.GetSection("Mem2").AsEnumerable(makePathsRelative: true).ToDictionary(k => k.Key, v => v.Value);
            Assert.AreEqual(3, dict2.Count);
            Assert.AreEqual("NoKeyValue2", dict2[""]);
            Assert.AreEqual("ValueInMem2", dict2["KeyInMem2"]);
            Assert.AreEqual("ValueDeep2", dict2["KeyInMem2:Deep2"]);

            var dict3 = config.GetSection("Mem3").AsEnumerable(makePathsRelative: true).ToDictionary(k => k.Key, v => v.Value);
            Assert.AreEqual(5, dict3.Count);
            Assert.AreEqual("NoKeyValue3", dict3[""]);
            Assert.AreEqual("ValueInMem3", dict3["KeyInMem3"]);
            Assert.AreEqual("ValueInMem4", dict3["KeyInMem4"]);
            Assert.AreEqual("ValueDeep3", dict3["KeyInMem3:Deep3"]);
            Assert.AreEqual("ValueDeep4", dict3["KeyInMem3:Deep4"]);
        }


        [TestMethod]
        public void NewConfigurationProviderOverridesOldOneWhenKeyIsDuplicated()
        {
            // Arrange
            var dic1 = new Dictionary<string, string>()
                {
                    {"Key1:Key2", "ValueInMem1"}
                };
            var dic2 = new Dictionary<string, string>()
                {
                    {"Key1:Key2", "ValueInMem2"}
                };
            var memConfigSrc1 = new MemoryConfigurationSource { InitialData = dic1 };
            var memConfigSrc2 = new MemoryConfigurationSource { InitialData = dic2 };

            var configurationBuilder = new SavableConfiurationBuilder();

            // Act
            configurationBuilder.Add(memConfigSrc1);
            configurationBuilder.Add(memConfigSrc2);

            var config = configurationBuilder.Build();

            // Assert
            Assert.AreEqual("ValueInMem2", config["Key1:Key2"]);
        }

        [TestMethod]
        public void NewConfigurationRootMayBeBuiltFromExistingWithDuplicateKeys()
        {
            var configurationRoot = new SavableConfiurationBuilder()
                                    .AddInMemoryCollection(new Dictionary<string, string>
                                        {
                                            {"keya:keyb", "valueA"},
                                        })
                                    .AddInMemoryCollection(new Dictionary<string, string>
                                        {
                                            {"KEYA:KEYB", "valueB"}
                                        })
                                    .Build();
            var newConfigurationRoot = new SavableConfiurationBuilder()
                .AddInMemoryCollection(configurationRoot.AsEnumerable())
                .Build();
            Assert.AreEqual("valueB", newConfigurationRoot["keya:keyb"]);
        }

        public class TestMemorySourceProvider : MemoryConfigurationProvider, IConfigurationSource
        {
            public TestMemorySourceProvider(Dictionary<string, string> initialData)
                : base(new MemoryConfigurationSource { InitialData = initialData })
            { }

            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                return this;
            }
        }

        [TestMethod]
        public void SettingValueUpdatesAllConfigurationProviders()
        {
            // Arrange
            var dict = new Dictionary<string, string>()
            {
                {"Key1", "Value1"},
                {"Key2", "Value2"}
            };

            var memConfigSrc1 = new TestMemorySourceProvider(dict);
            var memConfigSrc2 = new TestMemorySourceProvider(dict);
            var memConfigSrc3 = new TestMemorySourceProvider(dict);

            var configurationBuilder = new SavableConfiurationBuilder();

            configurationBuilder.Add(memConfigSrc1);
            configurationBuilder.Add(memConfigSrc2);
            configurationBuilder.Add(memConfigSrc3);

            var config = configurationBuilder.Build();

            // Act
            config["Key1"] = "NewValue1";
            config["Key2"] = "NewValue2";

            var memConfigProvider1 = memConfigSrc1.Build(configurationBuilder);
            var memConfigProvider2 = memConfigSrc2.Build(configurationBuilder);
            var memConfigProvider3 = memConfigSrc3.Build(configurationBuilder);

            // Assert
            Assert.AreEqual("NewValue1", config["Key1"]);
            Assert.AreEqual("NewValue1", memConfigProvider1.Get("Key1"));
            Assert.AreEqual("NewValue1", memConfigProvider2.Get("Key1"));
            Assert.AreEqual("NewValue1", memConfigProvider3.Get("Key1"));
            Assert.AreEqual("NewValue2", config["Key2"]);
            Assert.AreEqual("NewValue2", memConfigProvider1.Get("Key2"));
            Assert.AreEqual("NewValue2", memConfigProvider2.Get("Key2"));
            Assert.AreEqual("NewValue2", memConfigProvider3.Get("Key2"));
        }

        [TestMethod]
        public void CanGetConfigurationSection()
        {
            // Arrange
            var dic1 = new Dictionary<string, string>()
            {
                {"Data:DB1:Connection1", "MemVal1"},
                {"Data:DB1:Connection2", "MemVal2"}
            };
            var dic2 = new Dictionary<string, string>()
            {
                {"DataSource:DB2:Connection", "MemVal3"}
            };
            var dic3 = new Dictionary<string, string>()
            {
                {"Data", "MemVal4"}
            };
            var memConfigSrc1 = new MemoryConfigurationSource { InitialData = dic1 };
            var memConfigSrc2 = new MemoryConfigurationSource { InitialData = dic2 };
            var memConfigSrc3 = new MemoryConfigurationSource { InitialData = dic3 };

            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.Add(memConfigSrc1);
            configurationBuilder.Add(memConfigSrc2);
            configurationBuilder.Add(memConfigSrc3);

            var config = configurationBuilder.Build();

            // Act
            var configFocus = config.GetSection("Data");

            var memVal1 = configFocus["DB1:Connection1"];
            var memVal2 = configFocus["DB1:Connection2"];
            var memVal3 = configFocus["DB2:Connection"];
            var memVal4 = configFocus["Source:DB2:Connection"];
            var memVal5 = configFocus.Value;

            // Assert
            Assert.AreEqual("MemVal1", memVal1);
            Assert.AreEqual("MemVal2", memVal2);
            Assert.AreEqual("MemVal4", memVal5);

            Assert.AreEqual("MemVal1", configFocus["DB1:Connection1"]);
            Assert.AreEqual("MemVal2", configFocus["DB1:Connection2"]);
            Assert.IsNull(configFocus["DB2:Connection"]);
            Assert.IsNull(configFocus["Source:DB2:Connection"]);
            Assert.AreEqual("MemVal4", configFocus.Value);
            configFocus.Value = "Changed";
            Assert.AreEqual("Changed", configFocus.Value);
            configFocus["Dt"]= "ChangedFromThis";
            Assert.AreEqual("ChangedFromThis", configFocus["Dt"]);

            var section = configFocus.GetSection("TSection");
            Assert.IsTrue(section.Path.EndsWith("TSection"));

            var reloadToken = configFocus.GetReloadToken();
            Assert.IsNotNull(reloadToken);
        }

        [TestMethod]
        public void CanGetConnectionStrings()
        {
            // Arrange
            var dic1 = new Dictionary<string, string>()
            {
                {"ConnectionStrings:DB1:Connection1", "MemVal1"},
                {"ConnectionStrings:DB1:Connection2", "MemVal2"}
            };
            var dic2 = new Dictionary<string, string>()
            {
                {"ConnectionStrings:DB2:Connection", "MemVal3"}
            };
            var memConfigSrc1 = new MemoryConfigurationSource { InitialData = dic1 };
            var memConfigSrc2 = new MemoryConfigurationSource { InitialData = dic2 };

            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.Add(memConfigSrc1);
            configurationBuilder.Add(memConfigSrc2);

            var config = configurationBuilder.Build();

            // Act
            var memVal1 = config.GetConnectionString("DB1:Connection1");
            var memVal2 = config.GetConnectionString("DB1:Connection2");
            var memVal3 = config.GetConnectionString("DB2:Connection");

            // Assert
            Assert.AreEqual("MemVal1", memVal1);
            Assert.AreEqual("MemVal2", memVal2);
            Assert.AreEqual("MemVal3", memVal3);
        }

        [TestMethod]
        public void CanGetConfigurationChildren()
        {
            // Arrange
            var dic1 = new Dictionary<string, string>()
            {
                {"Data:DB1:Connection1", "MemVal1"},
                {"Data:DB1:Connection2", "MemVal2"}
            };
            var dic2 = new Dictionary<string, string>()
            {
                {"Data:DB2Connection", "MemVal3"}
            };
            var dic3 = new Dictionary<string, string>()
            {
                {"DataSource:DB3:Connection", "MemVal4"}
            };
            var memConfigSrc1 = new MemoryConfigurationSource { InitialData = dic1 };
            var memConfigSrc2 = new MemoryConfigurationSource { InitialData = dic2 };
            var memConfigSrc3 = new MemoryConfigurationSource { InitialData = dic3 };

            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.Add(memConfigSrc1);
            configurationBuilder.Add(memConfigSrc2);
            configurationBuilder.Add(memConfigSrc3);

            var config = configurationBuilder.Build();

            // Act
            var configSections = config.GetSection("Data").GetChildren().ToList();

            // Assert
            Assert.AreEqual(2, configSections.Count());
            Assert.AreEqual("MemVal1", configSections.FirstOrDefault(c => c.Key == "DB1")["Connection1"]);
            Assert.AreEqual("MemVal2", configSections.FirstOrDefault(c => c.Key == "DB1")["Connection2"]);
            Assert.AreEqual("MemVal3", configSections.FirstOrDefault(c => c.Key == "DB2Connection").Value);
            Assert.IsFalse(configSections.Exists(c => c.Key == "DB3"));
            Assert.IsFalse(configSections.Exists(c => c.Key == "DB3"));
        }

        [TestMethod]
        public void SourcesReturnsAddedConfigurationProviders()
        {
            // Arrange
            var dict = new Dictionary<string, string>()
            {
                {"Mem:KeyInMem", "MemVal"}
            };
            var memConfigSrc1 = new MemoryConfigurationSource { InitialData = dict };
            var memConfigSrc2 = new MemoryConfigurationSource { InitialData = dict };
            var memConfigSrc3 = new MemoryConfigurationSource { InitialData = dict };

            var srcSet = new HashSet<IConfigurationSource>()
            {
                memConfigSrc1,
                memConfigSrc2,
                memConfigSrc3
            };

            var configurationBuilder = new SavableConfiurationBuilder();

            // Act
            configurationBuilder.Add(memConfigSrc1);
            configurationBuilder.Add(memConfigSrc2);
            configurationBuilder.Add(memConfigSrc3);

            var config = configurationBuilder.Build();

            // Assert
            var includes = new[] { memConfigSrc1, memConfigSrc2, memConfigSrc3 };
            foreach (var item in includes)
            {
                if (!configurationBuilder.Sources.Any(x=>x==item))
                {
                    Assert.Fail($"{item} is not includes");
                }
            }
        }

        [TestMethod]
        public void SameReloadTokenIsReturnedRepeatedly()
        {
            // Arrange
            var configurationBuilder = new SavableConfiurationBuilder();
            var config = configurationBuilder.Build();

            // Act
            var token1 = config.GetReloadToken();
            var token2 = config.GetReloadToken();

            // Assert
            Assert.AreEqual(token1, token2);
        }

        [TestMethod]
        public void DifferentReloadTokenReturnedAfterReloading()
        {
            // Arrange
            var configurationBuilder = new SavableConfiurationBuilder();
            var config = configurationBuilder.Build();

            // Act
            var token1 = config.GetReloadToken();
            var token2 = config.GetReloadToken();
            config.Reload();
            var token3 = config.GetReloadToken();
            var token4 = config.GetReloadToken();

            // Assert
            Assert.AreSame(token1, token2);
            Assert.AreSame(token3, token4);
            Assert.AreNotSame(token1, token3);
        }

        [TestMethod]
        public void TokenTriggeredWhenReloadOccurs()
        {
            // Arrange
            var configurationBuilder = new SavableConfiurationBuilder();
            var config = configurationBuilder.Build();

            // Act
            var token1 = config.GetReloadToken();
            var hasChanged1 = token1.HasChanged;
            config.Reload();
            var hasChanged2 = token1.HasChanged;

            // Assert
            Assert.IsFalse(hasChanged1);
            Assert.IsTrue(hasChanged2);
        }

        [TestMethod]
        public void MultipleCallbacksCanBeRegisteredToReload()
        {
            // Arrange
            var configurationBuilder = new SavableConfiurationBuilder();
            var config = configurationBuilder.Build();

            // Act
            var token1 = config.GetReloadToken();
            var called1 = 0;
            token1.RegisterChangeCallback(_ => called1++, state: null);
            var called2 = 0;
            token1.RegisterChangeCallback(_ => called2++, state: null);

            // Assert
            Assert.AreEqual(0, called1);
            Assert.AreEqual(0, called2);

            config.Reload();
            Assert.AreEqual(1, called1);
            Assert.AreEqual(1, called2);

            var token2 = config.GetReloadToken();
            var cleanup1 = token2.RegisterChangeCallback(_ => called1++, state: null);
            token2.RegisterChangeCallback(_ => called2++, state: null);

            cleanup1.Dispose();

            config.Reload();
            Assert.AreEqual(1, called1);
            Assert.AreEqual(2, called2);
        }

        [TestMethod]
        public void NewTokenAfterReloadIsNotChanged()
        {
            // Arrange
            var configurationBuilder = new SavableConfiurationBuilder();
            var config = configurationBuilder.Build();

            // Act
            var token1 = config.GetReloadToken();
            var hasChanged1 = token1.HasChanged;
            config.Reload();
            var hasChanged2 = token1.HasChanged;
            var token2 = config.GetReloadToken();
            var hasChanged3 = token2.HasChanged;

            // Assert
            Assert.IsFalse(hasChanged1);
            Assert.IsTrue(hasChanged2);
            Assert.IsFalse(hasChanged3);
            Assert.AreNotSame(token1, token2);
        }

        [TestMethod]
        public void KeyStartingWithColonMeansFirstSectionHasEmptyName()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                [":Key2"] = "value"
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var children = config.GetChildren().ToArray();

            // Assert
            Assert.AreEqual(1,children.Length);
            Assert.AreEqual(string.Empty, children.First().Key);
            Assert.AreEqual(1,children.First().GetChildren().Count());
            Assert.AreEqual("Key2", children.First().GetChildren().First().Key);
        }

        [TestMethod]
        public void KeyWithDoubleColonHasSectionWithEmptyName()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Key1::Key3"] = "value"
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var children = config.GetChildren().ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            Assert.AreEqual("Key1", children.First().Key);
            Assert.AreEqual(1,children.First().GetChildren().Count());
            Assert.AreEqual(string.Empty, children.First().GetChildren().First().Key);
            Assert.AreEqual(1,children.First().GetChildren().First().GetChildren().Count());
            Assert.AreEqual("Key3", children.First().GetChildren().First().GetChildren().First().Key);
        }

        [TestMethod]
        public void KeyEndingWithColonMeansLastSectionHasEmptyName()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Key1:"] = "value"
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var children = config.GetChildren().ToArray();

            // Assert
            Assert.AreEqual(1,children.Length);
            Assert.AreEqual("Key1", children.First().Key);
            Assert.AreEqual(1,children.First().GetChildren().Count());
            Assert.AreEqual(string.Empty, children.First().GetChildren().First().Key);
        }

        [TestMethod]
        public void SectionWithValueExists()
        {
            // Arrange
            var dict = new Dictionary<string, string>()
            {
                {"Mem1", "Value1"},
                {"Mem1:KeyInMem1", "ValueInMem1"},
                {"Mem1:KeyInMem1:Deep1", "ValueDeep1"}
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var sectionExists1 = config.GetSection("Mem1").Exists();
            var sectionExists2 = config.GetSection("Mem1:KeyInMem1").Exists();
            var sectionNotExists = config.GetSection("Mem2").Exists();

            // Assert
            Assert.IsTrue(sectionExists1);
            Assert.IsTrue(sectionExists2);
            Assert.IsFalse(sectionNotExists);
        }

        [TestMethod]
        public void SectionGetRequiredSectionSuccess()
        {
            // Arrange
            var dict = new Dictionary<string, string>()
            {
                {"Mem1", "Value1"},
                {"Mem1:KeyInMem1", "ValueInMem1"},
                {"Mem1:KeyInMem1:Deep1", "ValueDeep1"}
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            IConfigurationRoot config = configurationBuilder.Build();
            
            // Act
            var sectionExists1 = config.GetRequiredSection("Mem1").Exists();
            var sectionExists2 = config.GetRequiredSection("Mem1:KeyInMem1").Exists();

            // Assert
            Assert.IsTrue(sectionExists1);
            Assert.IsTrue(sectionExists2);
        }

        [TestMethod]
        public void SectionGetRequiredSectionMissingThrowException()
        {
            // Arrange
            var dict = new Dictionary<string, string>()
            {
                {"Mem1", "Value1"},
                {"Mem1:Deep1", "Value1"},
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            IConfigurationRoot config = configurationBuilder.Build();

            Assert.ThrowsException<InvalidOperationException>(() => config.GetRequiredSection("Mem2"));
            Assert.ThrowsException<InvalidOperationException>(() => config.GetRequiredSection("Mem1:Deep2"));
        }

        [TestMethod]
        public void SectionGetRequiredSectionNullThrowException()
        {
            IConfigurationRoot config = null;
            Assert.ThrowsException<ArgumentNullException>(() => config.GetRequiredSection("Mem1"));
        }

        [TestMethod]
        public void SectionWithChildrenExists()
        {
            // Arrange
            var dict = new Dictionary<string, string>()
            {
                {"Mem1:KeyInMem1", "ValueInMem1"},
                {"Mem1:KeyInMem1:Deep1", "ValueDeep1"},
                {"Mem2:KeyInMem2:Deep1", "ValueDeep2"}
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var sectionExists1 = config.GetSection("Mem1").Exists();
            var sectionExists2 = config.GetSection("Mem2").Exists();
            var sectionNotExists = config.GetSection("Mem3").Exists();

            // Assert
            Assert.IsTrue(sectionExists1);
            Assert.IsTrue(sectionExists2);
            Assert.IsFalse(sectionNotExists);
        }

        [TestMethod]
        [DataRow("Value1")]
        [DataRow("")]
        public void KeyWithValueAndWithoutChildrenExistsAsSection(string value)
        {
            // Arrange
            var dict = new Dictionary<string, string>()
            {
                {"Mem1", value}
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var sectionExists = config.GetSection("Mem1").Exists();

            // Assert
            Assert.IsTrue(sectionExists);
        }

        [TestMethod]
        public void KeyWithNullValueAndWithoutChildrenIsASectionButNotExists()
        {
            // Arrange
            var dict = new Dictionary<string, string>()
            {
                {"Mem1", null}
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var sections = config.GetChildren();
            var sectionExists = config.GetSection("Mem1").Exists();
            var sectionChildren = config.GetSection("Mem1").GetChildren();

            // Assert
            Assert.AreEqual(1,sections.Count(section => section.Key == "Mem1"));
            Assert.IsFalse(sectionExists);
            Assert.AreEqual(0,sectionChildren.Count());
        }

        [TestMethod]
        public void SectionWithChildrenHasNullValue()
        {
            // Arrange
            var dict = new Dictionary<string, string>()
            {
                {"Mem1:KeyInMem1", "ValueInMem1"},
            };
            var configurationBuilder = new SavableConfiurationBuilder();
            configurationBuilder.AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var sectionValue = config.GetSection("Mem1").Value;

            // Assert
            Assert.IsNull(sectionValue);
        }

        [TestMethod]
        public void NullSectionDoesNotExist()
        {
            // Arrange
            // Act
            var sectionExists = ConfigurationExtensions.Exists(null);

            // Assert
            Assert.IsFalse(sectionExists);
        }
        [TestMethod]
        public void GetReloadToken_ActiveChangeCallbacks_MustTrue()
        {
            var root = new SavableConfigurationRoot(new IConfigurationProvider[0]);
            var tk = root.GetReloadToken();
            Assert.IsTrue(tk.ActiveChangeCallbacks);
        }
        [TestMethod]
        public void AddWithNull_MustThrowException()
        {
            var builder = new SavableConfiurationBuilder();
            Assert.ThrowsException<ArgumentNullException>(() => builder.Add(null));
        }
        [TestMethod]
        public void InitSavableConfigurationSectionWithNull_MustThrowException()
        {
            var root = new SavableConfigurationRoot(new List<IConfigurationProvider>());
            Assert.ThrowsException<ArgumentNullException>(() => new SavableConfigurationSection(null, "--path--"));
            Assert.ThrowsException<ArgumentNullException>(() => new SavableConfigurationSection(root,null));
            Assert.ThrowsException<ArgumentNullException>(() => new SavableConfigurationSection(null, null));
        }
        [TestMethod]
        public void InitSavableConfigurationRootWithNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>new SavableConfigurationRoot(null));
        }
        [TestMethod]
        public void GetValueFromZeroProviderSavableConfigurationRoot_MustThrowException()
        {
            var root = new SavableConfigurationRoot(new IConfigurationProvider[0]);
            Assert.ThrowsException<InvalidOperationException>(() =>root["a"]="a");
        }
        [TestMethod]
        public void Reload_AllMustReloaded()
        {
            var builder = new SavableConfiurationBuilder();
            builder.AddInMemoryCollection();
            var root = builder.Build();
            root.Reload();
        }
        [TestMethod]
        public void GetConfigWithSubscritedChanged_MustRaiseChanged()
        {
            var dic = new Dictionary<string, string>
            {
                ["hello"]="world"
            };
            var builder = new SavableConfiurationBuilder();
            builder.AddInMemoryCollection(dic);
            var root = builder.Build();
            IConfigurationChangeInfo info = null;
            root.ConfigurationChanged += (e) =>
            {
                info = e;
            };
            root["hello"] = "no-world";
            Assert.IsNotNull(info);
            Assert.AreEqual("hello", info.Key);
            Assert.AreEqual("world", info.Old);
            Assert.AreEqual("no-world", info.New);
            Assert.AreEqual(root, info.Sender);
            Assert.IsFalse(info.IsCreate);

            info = null;
            root["qaq"] = "qwq";
            Assert.IsNotNull(info);
            Assert.AreEqual("qaq", info.Key);
            Assert.IsNull(info.Old);
            Assert.AreEqual("qwq", info.New);
            Assert.AreEqual(root, info.Sender);
            Assert.IsTrue(info.IsCreate);

            root.Dispose();
        }

        internal class NullReloadTokenConfigSource : IConfigurationSource, IConfigurationProvider
        {
            public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath) => throw new NotImplementedException();
            public Primitives.IChangeToken GetReloadToken() => null;
            public void Load() { }
            public void Set(string key, string value) => throw new NotImplementedException();
            public bool TryGet(string key, out string value) => throw new NotImplementedException();
            public IConfigurationProvider Build(IConfigurationBuilder builder) => this;
        }

    }
}
