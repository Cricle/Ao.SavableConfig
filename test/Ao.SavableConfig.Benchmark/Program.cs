using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Saver;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
using System;

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
