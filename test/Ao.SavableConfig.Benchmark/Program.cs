using BenchmarkDotNet.Running;

namespace Ao.SavableConfig.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
                .Run();
        }
    }
}
