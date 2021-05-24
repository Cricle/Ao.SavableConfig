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
    public class ChangeSaverExtensionsTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ChangeSaverExtensions.EmitAndSave(null));
        }
        [TestMethod]
        public void CallEmitAndSave_MustReturnReport()
        {
            var reports = new ChangeReport[]
            {
                new ChangeReport(null,null,new IConfigurationChangeInfo[]
                {
                    new ConfigurationChangeInfo()
                })
            };
            var saver = new IChangeTransferCondition[]
            {
                new NullTransfer()
            };
            var rep = new ChangeSaver(reports, saver);
            var res = ChangeSaverExtensions.EmitAndSave(rep);
            Assert.IsNotNull(res);
        }
    }
}
