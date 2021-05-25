using Ao.SavableConfig.Saver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Test.Saver
{
    [TestClass]
    public class ChangeSaverTest
    {
        [TestMethod]
        public void InitWithNull_MustThrowException()
        {
            var reports = new ChangeReport[0];
            Assert.ThrowsException<ArgumentNullException>(() => new ChangeSaver(null));
            Assert.ThrowsException<ArgumentNullException>(() => new ChangeSaver(null,null));
            Assert.ThrowsException<ArgumentNullException>(() => new ChangeSaver(reports,null));
            Assert.ThrowsException<ArgumentNullException>(() => new ChangeSaver(reports, (IEnumerable<IChangeTransferCondition>)null));
            Assert.ThrowsException<ArgumentNullException>(() => new ChangeSaver(null, Enumerable.Empty<IChangeTransferCondition>()));
        }
        [TestMethod]
        public void PropertyValueMustEqualInput()
        {
            var reports = new ChangeReport[0];
            var condition = new IChangeTransferCondition[0];
            var saver = new ChangeSaver(reports,condition);
            Assert.AreEqual(reports, saver.Reports);
            Assert.AreEqual(condition, saver.Transfers);

            saver = new ChangeSaver(reports,(IEnumerable<IChangeTransferCondition>)condition);
            Assert.AreEqual(reports, saver.Reports);
            Assert.AreEqual(condition, saver.Transfers);
        }
        [TestMethod]
        public void EmitWithNoting_MutReturnEmptyDic()
        {
            var reports = new ChangeReport[0];
            var condition = new IChangeTransferCondition[0];
            var saver = new ChangeSaver(reports, condition);
            var map=saver.Emit();
            Assert.AreEqual(0, map.Count);
        }
        [TestMethod]
        public void EmitWithTransfer_MustReturnResult()
        {
            var reports = new ChangeReport[]
            {
                new ChangeReport(null,null,new IConfigurationChangeInfo[]
                {
                    new ConfigurationChangeInfo()
                })
            };
            var condition = new IChangeTransferCondition[] 
            {
                new NullTransfer()
            };
            var saver = new ChangeSaver(reports, condition);
            var map = saver.Emit();
            Assert.AreEqual(1, map.Count);
            Assert.IsTrue(map.ContainsKey(reports[0]));
            var res = map[reports[0]];
            Assert.AreEqual(map.Keys.First(), res.Report);
            Assert.AreEqual(condition[0], res.Transfer);
            Assert.AreEqual(condition[0], res.SelectCondition);
            Assert.AreEqual(string.Empty, res.Transfed);
        }
    }
    class NullTransfer : IChangeTransferCondition, IChangeTransfer
    {
        public IChangeTransfer GetTransfe(ChangeReport report)
        {
            return this;
        }

        public void Save(ChangeReport report, string transfed)
        {
            //Do nothing
        }

        public string Transfe(ChangeReport report)
        {
            return string.Empty;
        }
    }
}
