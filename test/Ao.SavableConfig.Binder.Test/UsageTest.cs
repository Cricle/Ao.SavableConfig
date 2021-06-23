using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class UsageTest
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task CreateClass_FileChanged_InstanceMustChanged(bool proxy)
        {
            var fi = new FileInfo("app1.json");
            if (fi.Exists)
            {
                fi.Delete();
            }
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(fi.FullName, true, true);
            var root = builder.BuildSavable();
            ThemeService inst = null;
            if (proxy)
            {
                inst = root.AutoCreateProxy<ThemeService>();
            }
            else
            {
                inst = new ThemeService();
            }
            root.BindNotifyTwoWay(inst, JsonChangeTransferCondition.Instance);

            var ser = new ThemeService
            {
                Age = 22,
                ButtonEnable = true,
                Title = "title",
                ButtonStyle = new ButtonStyle
                {
                    Background = "bg",
                    ObjectStyle = new ObjectStyle
                    {
                        Order = 2
                    }
                },
                WindowStyle = Styles.SingleButton
            };
            var str = JsonConvert.SerializeObject(ser);
            File.WriteAllText(fi.FullName, str);

            await Task.Delay(1000);

            Assert.AreEqual(22, inst.Age);
            Assert.IsTrue(inst.ButtonEnable);
            Assert.AreEqual("title", inst.Title);
            Assert.AreEqual("bg", inst.ButtonStyle.Background);
            Assert.AreEqual(2, inst.ButtonStyle.ObjectStyle.Order);
            Assert.AreEqual(Styles.SingleButton, inst.WindowStyle);
        }
        [TestMethod]
        public async Task ProxyClass_ChangeProperty_FileMustChanged()
        {
            var fi = new FileInfo("app.json");
            if (fi.Exists)
            {
                fi.Delete();
            }
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(fi.FullName, true, true);
            var root = builder.BuildSavable();
            var inst = root.AutoCreateProxy<ThemeService>();
            root.BindNotifyTwoWay(inst, JsonChangeTransferCondition.Instance);

            inst.Age = 123;
            inst.ButtonEnable = true;
            inst.ButtonStyle.Background = "red";
            inst.ButtonStyle.ObjectStyle.Order = 123;

            await Task.Delay(TimeSpan.FromTicks(BindSettings.DefaultDelayTime.Ticks * 4));

            var content = File.ReadAllText(fi.FullName);
            var obj = JsonConvert.DeserializeObject<ThemeService>(content);
            Assert.AreEqual(123, obj.Age);
            Assert.IsTrue(obj.ButtonEnable);
            Assert.AreEqual("red", obj.ButtonStyle.Background);
            Assert.AreEqual(123, obj.ButtonStyle.ObjectStyle.Order);

            fi.Delete();
        }
    }
}
