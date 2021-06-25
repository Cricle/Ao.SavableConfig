using System;
using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Binder;
using BenchmarkDotNet.Attributes;
using Ao.SavableConfig.Binder.Visitors;

namespace Ao.SavableConfig.Benchmark
{
    [ConfigStepIn]
    public class StudentClass
    {
        public virtual string Name0 { get; set; }
        public virtual string Name1 { get; set; }
        public virtual string Name2 { get; set; }
        public virtual string Name3 { get; set; }

        public Student Student1 { get; set; }
        public Student Student2 { get; set; }
        public Student Student3 { get; set; }
        public Student Student4 { get; set; }
    }
    public class Student
    {
        public virtual string Name0 { get; set; }
        public virtual string Name1 { get; set; }
        public virtual string Name2 { get; set; }
        public virtual string Name3 { get; set; }
        public virtual string Name4 { get; set; }
    }
    [MemoryDiagnoser]
    public class NamedAnalysis
    {
        private INameTransfer nameTransfer;
        private Type type;
        private ObjectNamedCreator objectNamedCreator;
        public NamedAnalysis()
        {
            type = typeof(StudentClass);
            nameTransfer = IdentityMapNameTransfer.FromTypeAttributes(type);
            objectNamedCreator = new ObjectNamedCreator(type, nameTransfer,
                IdentityNamedCreator.Instance, ReflectionPropertyVisitor.Instance);

        }
        [Benchmark]
        public void Analysis()
        {
            objectNamedCreator.Analysis();
        }
    }
}
