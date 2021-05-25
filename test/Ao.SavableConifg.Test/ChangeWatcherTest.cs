﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Test
{
    [TestClass]
    public class ChangeWatcherTest
    {
        [TestMethod]
        public void Given_NullInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ChangeWatcher(null));
        }
        private SavableConfigurationRoot CreateRoot()
        {
            var builder = new SavableConfiurationBuilder();
            builder.AddInMemoryCollection();
            return builder.Build();
        }
        [TestMethod]
        public void AddChanged_MustStored()
        {
            var root = CreateRoot();
            var cw = new ChangeWatcher(root);
            Assert.AreEqual(root, cw.Configuration);
            root["hello"] = "world";
            Assert.AreEqual(1, cw.ChangeInfos.Count);
        }
        [TestMethod]
        public void AddChanged_ClearIt_MustClean()
        {
            var root = CreateRoot();
            var cw = new ChangeWatcher(root);
            root["hello"] = "world";
            cw.Clear();
            Assert.AreEqual(0, cw.ChangeInfos.Count);
        }
        [TestMethod]
        public void AddSameChanged_MustMerged()
        {
            var root = CreateRoot();
            var cw = new ChangeWatcher(root);
            Assert.AreEqual(root, cw.Configuration);
            root["hello"] = "world";
            root["hello"] = "world1";
            Assert.AreEqual(1, cw.ChangeInfos.Count);
            Assert.AreEqual(1, cw.ChangeCount);
            Assert.AreEqual("world1", cw.ChangeInfos[0].New);
        }
        [TestMethod]
        public void ShopIt_AddChange_MustDoNothing()
        {
            var root = CreateRoot();
            var cw = new ChangeWatcher(root);
            Assert.AreEqual(root, cw.Configuration);
            cw.Stop();
            root["hello"] = "world";
            Assert.AreEqual(0, cw.ChangeInfos.Count);
        }
        [TestMethod]
        public void DisposeIt_AddChange_MustDoNothing()
        {
            var root = CreateRoot();
            var cw = new ChangeWatcher(root);
            Assert.AreEqual(root, cw.Configuration);
            cw.Dispose();
            root["hello"] = "world";
            Assert.AreEqual(0, cw.ChangeInfos.Count);
        }
        [TestMethod]
        public void IgnoreSameTrue_AddChange_TwiceMustIgnored()
        {
            var root = CreateRoot();
            var cw = new ChangeWatcher(root);
            cw.IgnoreSame = true;
            root["hello"] = "world";
            root["hello"] = "world";
            Assert.AreEqual("world", cw.ChangeInfos[0].New);
            Assert.AreEqual(1, cw.ChangeCount);
        }
        [TestMethod]
        public void DoAnything_EventMustBeFired()
        {
            var root = CreateRoot();
            var cw = new ChangeWatcher(root);
            IConfigurationChangeInfo info = null;
            cw.ChangePushed += (o, e) =>
            {
                info = e;
            };
            root["a"] = "b";
            Assert.IsNotNull(info);
            EventArgs changeMgrArg = null;
            cw.ChangeMerged += (o, e) =>
            {
                changeMgrArg = e;
            };
            cw.Merge();
            Assert.IsNotNull(changeMgrArg);
            EventArgs changeClearedArg = null;
            cw.ChangeCleared += (o, e) =>
            {
                changeClearedArg = e;
            };
            cw.Clear();
            Assert.IsNotNull(changeClearedArg);
        }
    }
}