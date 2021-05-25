using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Test.Saver
{
    [TestClass]
    public class ChangeReportTest
    {
        [TestMethod]
        public void Init_PropertyValueMustEqualInput()
        {
            var root = new ConfigurationRoot(new IConfigurationProvider[0]);
            var infos = new IConfigurationChangeInfo[]
            {
                new ConfigurationChangeInfo()
            };
            var rep = ChangeReport.FromChanges(root,infos);
            Assert.AreEqual(1, rep.Count());
            var changes = rep.First();
            Assert.IsNull(changes.Provider);
            Assert.AreEqual(root, changes.Configuration);
            Assert.AreEqual(1,changes.IncludeChangeInfo.Count);
            Assert.IsTrue(changes.IncludeChangeInfo.Any(x=>x==infos[0]));
        }
        [TestMethod]
        public void GivenNotSameProvider_MustThrowException()
        {
            var root = new ConfigurationRoot(new IConfigurationProvider[0]);
            var provider1 = new NullConfigurationProvider();
            var provider2 = new NullConfigurationProvider();
            var infos = new IConfigurationChangeInfo[]
            {
                new ConfigurationChangeInfo
                {
                     Provider=provider1
                }
            };
            Assert.ThrowsException<ArgumentException>(() => new ChangeReport(root, provider2, infos));

        }
        [TestMethod]
        [DataRow("hello",null, ConfigurationTypes.Null,false)]
        [DataRow("hello", "1", ConfigurationTypes.Number, false)]
        [DataRow("hello", "false", ConfigurationTypes.Boolean, false)]
        [DataRow("hello", "true", ConfigurationTypes.Boolean, false)]
        [DataRow("hello", "1.123", ConfigurationTypes.Single, false)]
        [DataRow("hello", "dsagkj@#", ConfigurationTypes.String, false)]
        [DataRow("hello:0", "aa", ConfigurationTypes.Array, true)]
        public void Init_GetVarableReport(string name,string val, ConfigurationTypes type,bool isArray)
        {
            var root = new ConfigurationRoot(new IConfigurationProvider[0]);
            var infos = new IConfigurationChangeInfo[]
            {
                new ConfigurationChangeInfo
                {
                    Key=name,
                    Old="1",
                    New=val,
                    Sender=root,
                    Provider=null,
                    IsCreate=false
                }
            };
            var rep = ChangeReport.FromChanges(root, infos);
            var c = rep.First().GetValueReport();
            Assert.AreEqual(name, c.Keys.Single());
            var r = c.Values.Single();
            Assert.AreEqual(isArray,r.IsArray);
            Assert.AreEqual(type,r.TypeCode);
            Assert.AreEqual(root, r.Configuration);
            Assert.AreEqual(infos[0], r.Info);
        }
    }
    class NullConfigurationProvider : IConfigurationProvider
    {
        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            yield break;
        }

        public IChangeToken GetReloadToken()
        {
            return null;
        }

        public void Load()
        {
            //Do nothing
        }

        public void Set(string key, string value)
        {
            //Do nothing
        }

        public bool TryGet(string key, out string value)
        {
            value = null;
            return false;
        }
    }
}
