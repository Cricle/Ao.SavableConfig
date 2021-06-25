using Ao.SavableConfig.Binder;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Benchmark
{
    [MemoryDiagnoser]
    //[SimpleJob(RunStrategy.ColdStart)]
    public class ProxyCreat
    {
        private IConfiguration configuration;
        private readonly ProxyCreator proxyCreator;
        public ProxyCreat()
        {
            proxyCreator = ProxyHelper.Default.CreateComplexProxy<DbConnection>();
            ProxyHelper.Default.BuildProx(typeof(MConnection));
            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
        }
        [Benchmark]
        public void ProxySimple()
        {
            ProxyHelper.Default.BuildProx(typeof(MConnection));
        }
        [Benchmark]
        public void ProxyComplex()
        {
            ProxyHelper.Default.CreateComplexProxy<DbConnection>();
        }
        [Benchmark]
        public void CreateProxySimple()
        {
            ProxyHelper.Default.CreateProxy(typeof(MConnection), configuration, NullNameTransfer.Instance);
        }
        [Benchmark]
        public void CreateProxyComplex()
        {
            proxyCreator.Build(configuration);
        }
    }
}
