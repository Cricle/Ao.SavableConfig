using Ao.SavableConfig.Saver;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Binder.Test
{
    [TestClass]
    public class ObservableBindBoxTest
    {
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var setting = new BindSettings(new object(), TimeSpan.FromSeconds(1), new IChangeTransferCondition[0]);
            Assert.ThrowsException<ArgumentNullException>(() => new ObservableBindBox(root, null, setting, ConfigBindMode.TwoWay, a => a()));
        }
        [TestMethod]
        public void GivenNoNotifyObject_MustThrowException()
        {
            var root = ConfigHelper.CreateEmptyRoot();
            var setting = new BindSettings(new object(), TimeSpan.FromSeconds(1), new IChangeTransferCondition[0]);
            Assert.ThrowsException<InvalidCastException>(() => new ObservableBindBox(root, root, setting, ConfigBindMode.TwoWay, a => a()));
        }
        class Class : ObservableObject
        {
            private Student student1;
            private Student student2;
            private int count;

            public int Count
            {
                get => count;
                set => Set(ref count, value);
            }


            public Student Student1
            {
                get => student1;
                set => Set(ref student1, value);
            }
            public Student Student2
            {
                get => student2;
                set => Set(ref student2, value);
            }
        }
        class Student : ObservableObject
        {
            private string name;
            private int age;

            public string Name
            {
                get => name;
                set => Set(ref name, value);
            }
            public int Age
            {
                get => age;
                set => Set(ref age, value);
            }
        }
        [TestMethod]
        public void GivenNotifyObject_MustParsed()
        {
            var stu = new Student();
            var root = ConfigHelper.CreateEmptyRoot();
            var setting = new BindSettings(stu, TimeSpan.FromSeconds(1), new IChangeTransferCondition[0]);
            var box = new ObservableBindBox(root, root, setting, ConfigBindMode.TwoWay, a => a());
            Assert.IsNotNull(box.PropertyConfigMap);
            Assert.AreEqual(root, box.Configuration);
            Assert.AreEqual(stu, box.NotifyObject);
            Assert.AreEqual(2, box.NotifyPropertyMap.Count);
            Assert.IsTrue(box.NotifyPropertyMap.ContainsKey(nameof(Student.Name)));
            Assert.IsTrue(box.NotifyPropertyMap.ContainsKey(nameof(Student.Age)));

            box = new ObservableBindBox(root, setting, ConfigBindMode.TwoWay, a => a());
            Assert.AreEqual(root, box.Configuration);
            Assert.AreEqual(stu, box.NotifyObject);
            Assert.AreEqual(2, box.NotifyPropertyMap.Count);
            Assert.IsTrue(box.NotifyPropertyMap.ContainsKey(nameof(Student.Name)));
            Assert.IsTrue(box.NotifyPropertyMap.ContainsKey(nameof(Student.Age)));
        }
        private SavableConfigurationRoot CreateRoot()
        {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Student1:Name"] = "sn1",
                ["Student1:Age"] = "1",
                ["Student2:Name"] = "sn2",
                ["Student2:Age"] = "2",
            });
            var root = builder.BuildSavable();
            return root;
        }
        private async Task TestingBindTwoWayObserable(ObservableBindBoxUpdateModes updateMode)
        {

            var root = CreateRoot();

            var val = new Class();
            var setting = new BindSettings(val, TimeSpan.FromMilliseconds(100), new IChangeTransferCondition[0]);
            var box = new ObservableBindBox(root, root, setting, ConfigBindMode.TwoWay, a => a());
            box.UpdateMode = updateMode;
            box.Bind();

            var old = val.Student1;

            val.Student1 = new Student();

            val.Student1.Age = 777;

            Assert.AreEqual("777", root["Student1:Age"]);

            await Task.Delay(500);

            root["Student1:Age"] = "888";

            await Task.Delay(500);

            Assert.AreEqual(888, val.Student1.Age);
            Assert.AreNotEqual(888, old.Age);

            old.Age = 987;

            Assert.AreNotEqual("987", root["Student1:Age"]);
        }
        [TestMethod]
        public Task BindTwoWay_ReloadMode_ClassValueChange_MustDetchOld_AttackNew()
        {
            return TestingBindTwoWayObserable(ObservableBindBoxUpdateModes.Reload);
        }
        [TestMethod]
        public Task BindTwoWay_FindUpdateMode_ClassValueChange_MustDetchOld_AttackNew()
        {
            return TestingBindTwoWayObserable(ObservableBindBoxUpdateModes.FindUpdate);
        }
        private async Task TestingBindTwoWay(ObservableBindBoxUpdateModes updateMode)
        {

            var root = CreateRoot();

            var val = new Class();
            var setting = new BindSettings(val, TimeSpan.FromMilliseconds(200), new IChangeTransferCondition[0]);
            var box = new ObservableBindBox(root, root, setting, ConfigBindMode.TwoWay, a => a());
            box.UpdateMode = updateMode;
            box.Bind();

            Assert.AreEqual(val.Student1.Name, "sn1");
            Assert.AreEqual(val.Student1.Age, 1);

            Assert.AreEqual(val.Student2.Name, "sn2");
            Assert.AreEqual(val.Student2.Age, 2);

            val.Student1.Name = "sns1";

            Assert.AreEqual(val.Student1.Name, root["Student1:Name"]);
            await Task.Delay(500);

            root["Count"] = "222";
            await Task.Delay(500);
            Assert.AreEqual(222, val.Count);

            root["Student1:Name"] = "wwr";
            await Task.Delay(500);
            Assert.AreEqual(val.Student1.Name, root["Student1:Name"]);

            box.UnBind();
            val.Student1.Name = "snsqqq1";
            Assert.AreNotEqual(val.Student1.Name, root["Student1:Name"]);
            root["Student1:Name"] = "123";
            await Task.Delay(500);
            Assert.AreNotEqual(val.Student1.Name, root["Student1:Name"]);

            box.Dispose();
        }
        [TestMethod]
        public Task BindTwoWay_FindUpdateMode_ValueMustSeted_UnBind_WasDetched()
        {
            return TestingBindTwoWay(ObservableBindBoxUpdateModes.FindUpdate);
        }
        [TestMethod]
        public Task BindTwoWay_ReloadMode_ValueMustSeted_UnBind_WasDetched()
        {
            return TestingBindTwoWay(ObservableBindBoxUpdateModes.Reload);
        }
    }
}
