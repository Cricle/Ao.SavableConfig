using Ao.SavableConfig.Saver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Primitives;

namespace Ao.SavableConfig.Json.Test
{
    [TestClass]
    public class JsonChangeTransferConditionTest
    {
        private static string SettingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");
        private SavableConfigurationRoot CreateRoot(out string fn, string ext = "json")
        {
            fn = $"Setting1{Guid.NewGuid()}." + ext;
            var targetFile = Path.Combine(SettingPath, "Setting1.json");
            var destFile = Path.Combine(SettingPath, fn);
            File.Copy(targetFile, destFile);
            var jsonProvider = new JsonConfigurationSource
            {
                FileProvider = new PhysicalFileProvider(SettingPath),
                Optional = false,
                Path = fn,
            };
            var builder = new SavableConfiurationBuilder();
            builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            builder.Add(jsonProvider);
            var root = builder.Build();
            return root;
        }
        [TestMethod]
        public void GetTwiceInstance_MustEqual()
        {
            var a = JsonChangeTransferCondition.Instance;
            var b = JsonChangeTransferCondition.Instance;
            Assert.AreEqual(a, b);
        }
        class NullConfigurationProvider : IConfigurationProvider
        {
            public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
            {
                return null;
            }

            public IChangeToken GetReloadToken()
            {
                return null;
            }

            public void Load()
            {
            }

            public void Set(string key, string value)
            {
            }

            public bool TryGet(string key, out string value)
            {
                value = null;
                return false;
            }
        }
        [TestMethod]
        public void TransferNoJson_MustReturnNull()
        {
            var root = CreateRoot(out var fn);
            var condition = new JsonChangeTransferCondition();
            var prov = new NullConfigurationProvider();
            var rep = new ChangeReport(root, prov, new ConfigurationChangeInfo[0]);
            var val=condition.GetTransfe(rep);
            Assert.IsNull(val);
            var f = new JsonConfigurationProvider(new JsonConfigurationSource
            {
                 Path="a.xml"
            });
            rep = new ChangeReport(root, f, new ConfigurationChangeInfo[0]);
            val = condition.GetTransfe(rep);
            Assert.IsNull(val);
        }
        [TestMethod]
        public void TransferChanged_SaveIt_ButFileNotFound_MustWrited()
        {
            var root = CreateRoot(out var fn);
            File.Delete(Path.Combine(SettingPath,fn));
            var condition = new JsonChangeTransferCondition();
            var prov = root.Providers.First();
            var changeInfos = new ConfigurationChangeInfo[]
            {
                new ConfigurationChangeInfo
                {
                    IsCreate = false,
                    Key = "Title",
                    New = "world",
                    Old = "Hello",
                    Provider=prov,
                     Sender=root
                }
            };
            var rep = new ChangeReport(root, prov, changeInfos);
            var transfer = condition.GetTransfe(rep);
            Assert.IsNotNull(transfer);
            var t = transfer.Transfe(rep);
        }

        [TestMethod]
        public void TransferChanged_SaveIt_MustWrited()
        {
            var root = CreateRoot(out var fn);
            var condition = new JsonChangeTransferCondition();
            var prov = root.Providers.First();
            var changeInfos = new ConfigurationChangeInfo[]
            {
                new ConfigurationChangeInfo
                {
                    IsCreate = false,
                    Key = "Title",
                    New = "world",
                    Old = "Hello",
                    Provider=prov,
                     Sender=root
                }
            };
            var rep = new ChangeReport(root, prov, changeInfos);
            var transfer = condition.GetTransfe(rep);
            Assert.IsNotNull(transfer);
            var t = transfer.Transfe(rep);
            condition.Save(rep, t);

            var destFile = File.ReadAllText(Path.Combine(SettingPath, fn));
            var jobj = JObject.Parse(destFile);
            var oldTitle = jobj["Title"];
            Assert.AreEqual("world", oldTitle.ToString());
        }
    }
}
