using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Binder.Visitors;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Ao.SavableConfig.Benchmark
{
    [MemoryDiagnoser]
    public class NamedBuild
    {
        private ObjectNamedCreator objectNamedCreatorRef;
        private ObjectNamedCreator objectNamedCreatorCom;
        private object valueReflection;
        private object valueCompiled;
        private IConfiguration config;

        public NamedBuild()
        {
            var type = typeof(StudentClass);
            var nameTransfer = IdentityMapNameTransfer.FromTypeAttributes(type);
            var builder = new ConfigurationBuilder();
            var map = new Dictionary<string, string>
            {
                ["Name0"] = "1",
                ["Name1"] = "2",
                ["Name2"] = "3",
                ["Name3"] = "4",
                ["Name4"] = "5"
            };
            for (int i = 1; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    map[$"Student" + i + ":" + "Student" + j] = (i * j + j).ToString();
                }
            }
            builder.AddInMemoryCollection(map);
            config = builder.BuildSavable();

            objectNamedCreatorRef = new ObjectNamedCreator(type, nameTransfer,
                IdentityNamedCreator.Instance, ReflectionPropertyVisitor.Instance);
            valueReflection = objectNamedCreatorRef.Build(config);
            valueReflection = objectNamedCreatorRef.Build(valueReflection, config);

            objectNamedCreatorCom = new ObjectNamedCreator(type, nameTransfer,
                IdentityNamedCreator.Instance, ReflectionPropertyVisitor.Instance);
            valueCompiled = objectNamedCreatorCom.Build(config);
            valueCompiled = objectNamedCreatorCom.Build(valueCompiled, config);
        }
        private const int opCount = 100;
        [Benchmark(Baseline = true, OperationsPerInvoke = opCount)]
        public void BindRef()
        {
            for (int i = 0; i < opCount; i++)
            {
                objectNamedCreatorRef.Build(valueReflection, config);
            }
        }
        [Benchmark(OperationsPerInvoke = opCount)]
        public void BindCom()
        {
            for (int i = 0; i < opCount; i++)
            {
                objectNamedCreatorCom.Build(valueCompiled, config);
            }
        }
    }
}
