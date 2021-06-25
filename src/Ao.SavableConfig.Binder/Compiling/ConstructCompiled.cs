using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public delegate object ObjectCreator(params object[] @params);
    public class ConstructCompiled : IEquatable<ConstructCompiled>
    {
        private static readonly Type ObjectArrayType = typeof(object[]);

        public ConstructCompiled(ConstructorInfo constructor)
        {
            Constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            Creator = BuildCreate();
        }

        public ConstructorInfo Constructor { get; }

        public ObjectCreator Creator { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConstructCompiled);
        }
        public override int GetHashCode()
        {
            return Constructor.GetHashCode();
        }
        public bool Equals(ConstructCompiled other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Constructor == Constructor;
        }

        protected ObjectCreator BuildCreate()
        {
            var constPars = Constructor.GetParameters();
            var par = Expression.Parameter(ObjectArrayType);

            var convs = new Expression[constPars.Length];
            for (int i = 0; i < constPars.Length; i++)
            {
                var arg = Expression.Convert(
                    Expression.ArrayIndex(par, Expression.Constant(i)),
                        constPars[i].ParameterType);
                convs[i] = arg;
            }

            var newBlock = Expression.New(Constructor, convs);

            var lambda = Expression.Lambda<ObjectCreator>(newBlock,
                par).Compile();

            return lambda;
        }
    }
}
