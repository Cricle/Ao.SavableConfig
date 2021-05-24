using Ao.SavableConfig.Binder;
using BenchmarkDotNet.Attributes;

namespace Ao.SavableConfig.Benchmark
{
    [MemoryDiagnoser]
    public class ProxyCreat
    {
        [Benchmark]
        public void ProxySimple()
        {
            ProxyHelper.Default.BuildProx(typeof(MConnection));
        }
        [Benchmark]
        public void ProxyComplex()
        {
            ProxyHelper.Default.CreateComplexProxy<DbConnection>(true);
        }
    }
}
