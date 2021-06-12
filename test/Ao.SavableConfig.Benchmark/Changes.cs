using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart)]
    public class Changes
    {
        private const int LoopCount = 1000;
        private readonly SavableConfigurationRoot root;
        private readonly SavableConfigurationRoot noListenRoot;
        private readonly IConfiguration msroot;
        private readonly ChangeWatcher changeWatcher;
        public Changes()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", true, false);
            root = builder.BuildSavable();
            changeWatcher = new ChangeWatcher(root);

            var noListenBuilder = new ConfigurationBuilder();
            noListenBuilder.AddJsonFile("appsettings.json", true, false);
            noListenRoot = noListenBuilder.BuildSavable();


            var msbuilder = new ConfigurationBuilder();
            msbuilder.AddJsonFile("appsettings.json", true, false);
            msroot = msbuilder.Build();
        }
        [Benchmark(Baseline =true,OperationsPerInvoke =LoopCount)]
        public void MsChange()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                msroot["DbConnections:Mysql:Connection"+i] = i.ToString();
            }
        }
        [Benchmark(OperationsPerInvoke = LoopCount)]
        public void SavableChange()
        {
            changeWatcher.Clear();
            for (int i = 0; i < LoopCount; i++)
            {
                root["DbConnections:Mysql:Connection" + i] = i.ToString();
            }
            changeWatcher.Merge();
        }
        [Benchmark(OperationsPerInvoke = LoopCount)]
        public void NoListeneAndSavableChange()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                noListenRoot["DbConnections:Mysql:Connection" + i] = i.ToString();
            }
        }
    }
}
