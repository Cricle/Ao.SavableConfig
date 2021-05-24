using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Test
{
    [TestClass]
    public class ConcurrentOnceTest
    {
        [TestMethod]
        public async Task RunOnce_MustReturnTrue()
        {
            var once = new ConcurrentOnce();
            var ok=await once.WaitAsync(TimeSpan.Zero);
            Assert.IsTrue(ok);

            once.ToString();
        }
        [TestMethod]
        public async Task RunWhenConcurrent_PrevMustFail()
        {
            var once = new ConcurrentOnce();
            var ok1 = once.WaitAsync(TimeSpan.FromSeconds(1));
            var ok2 = await once.WaitAsync(TimeSpan.Zero);
            Assert.IsTrue(ok2);
            Assert.IsFalse(await ok1);
        }
        [TestMethod]
        public async Task RunAfter_TokenMustChanged()
        {
            var once = new ConcurrentOnce();
            var tk1 = once.Token;
            await once.WaitAsync(TimeSpan.Zero);
            var tk2 = once.Token;
            Assert.AreNotEqual(tk1, tk2);
        }
    }
}
