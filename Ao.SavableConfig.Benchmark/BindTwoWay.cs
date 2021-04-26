using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Saver;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Benchmark
{
    [MemoryDiagnoser]
    public class BindTwoWay
    {
        private readonly SavableConfigurationRoot root;
        private readonly TwoWayDbConnection conn;

        [Params(10,100,1000)]
        public int Count { get; set; }

        public BindTwoWay()
        {
            var builder = new SavableConfiurationBuilder();
            builder.AddJsonFile("appsettings.json", true, true);
            root = builder.Build();
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
