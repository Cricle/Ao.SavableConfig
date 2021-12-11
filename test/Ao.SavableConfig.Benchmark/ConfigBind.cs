using Ao.SavableConfig.Binder;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Ao.SavableConfig.Benchmark
{
    [MemoryDiagnoser]
    public class ConfigBind
    {
        private IConfiguration config;

        [GlobalSetup]
        public void Init()
        {
            var builder = new ConfigurationBuilder();
            var map = new Dictionary<string, string>
            {
                ["Name0"] = "1",
                ["Name1"] = "2",
                ["Name2"] = "3",
                ["Name3"] = "4",
                ["Name4"] = "5",
                ["Name:0"] = "1",
                ["Name:1"] = "1",
                ["Name:2"] = "1",
                ["Name:3"] = "1",
                ["Name:4"] = "1",
            };
            for (int i = 1; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    map[$"Student" + i + ":" + "Student" + j] = (i * j + j).ToString();
                }
            }
            builder.AddInMemoryCollection(map);
            config = builder.Build();

            GetToType();
            FastGetToType();
        }

        [Params(1,1000)]
        public int Count { get; set; }

        [Benchmark]
        public void GetToType()
        {
            var names = new Names();
            for (int i = 0; i < Count; i++)
            {
                ConfigurationBinder.Bind(config, names);
            }
        }
        [Benchmark]
        public void FastGetToType()
        {
            var names = new Names();
            for (int i = 0; i < Count; i++)
            {
                FastConfigurationBinder.FastBind(config, names);
            }
        }
        [Benchmark]
        public void GetToList()
        {
            for (int i = 0; i < Count; i++)
            {
                ConfigurationBinder.GetValue<string[]>(config, "Name");
            }
        }
        [Benchmark]
        public void FastGetToList()
        {
            for (int i = 0; i < Count; i++)
            {
                FastConfigurationBinder.FastGetValue<string[]>(config, "Name");
            }
        }
    }
}
