using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Benchmark
{
    [MemoryDiagnoser]
    public class Changes
    {
        private const int LoopCount = 1000;
        private readonly SavableConfigurationRoot root;
        private readonly IConfiguration msroot;
        private readonly ChangeWatcher changeWatcher;
        public Changes()
        {
            var builder = new SavableConfiurationBuilder();
            builder.AddJsonFile("appsettings.json", true, false);
            root = builder.Build();
            changeWatcher = new ChangeWatcher(root);

            var msbuilder = new ConfigurationBuilder();
            msbuilder.AddJsonFile("appsettings.json", true, false);
            msroot = msbuilder.Build();
        }
        [Benchmark(Baseline =true,OperationsPerInvoke =LoopCount)]
        public void MsChange()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                msroot["DbConnections:Mysql:Connection"] = i.ToString();
            }
        }
        [Benchmark(OperationsPerInvoke = LoopCount)]
        public void SavableChange()
        {
            changeWatcher.Clear();
            for (int i = 0; i < LoopCount; i++)
            {
                root["DbConnections:Mysql:Connection"] = i.ToString();
            }
        }
    }
}
