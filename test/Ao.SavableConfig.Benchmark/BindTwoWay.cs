using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Saver;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart)]
    public class BindTwoWay
    {
        private readonly TwoWayDbConnection conn;

        [Params(10,100,1000)]
        public int Count { get; set; }

        public BindTwoWay()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", true, true);
            var root = builder.BuildSavable();
            conn = root.CreateComplexProxy<TwoWayDbConnection>();
            root.BindTwoWay(conn, JsonChangeTransferCondition.Instance);
        }
        [Benchmark]
        public void TwoWayChange()
        {
            for (int i = 0; i < Count; i++)
            {
                conn.Mssql.Connection = i.ToString();
            }
        }
        
    }
}
