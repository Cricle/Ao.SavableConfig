
using BenchmarkDotNet.Attributes;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Benchmark
{
    [MemoryDiagnoser]
    public class ObservableBindBoxChanges
    {
        private readonly SavableConfigurationRoot root;
        private readonly Student student;
        private readonly Class @class;
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

        public ObservableBindBoxChanges()
        {
            root = new ConfigurationBuilder().AddInMemoryCollection().BuildSavable();
            student = new Student();
            @class = new Class();
            root.BindNotifyTwoWay(student);
            root.BindNotifyTwoWay(@class);
        }
        [Benchmark]
        public void CLRChange()
        {
            student.Age = 123;
            student.Name = "n1";
        }
        [Benchmark]
        public void ConfigChange()
        {
            root["Age"] = "456";
            root["Name"] = "q1";
        }
    }
}
