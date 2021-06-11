using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class PropertyCompiled : IEquatable<PropertyCompiled>
    {
        private static readonly Type ObjectType = typeof(object);

        public PropertyCompiled(PropertyInfo member)
        {
            Member = member ?? throw new ArgumentNullException(nameof(member));

            Setter = BuildSetter();
            Getter = BuildGetter();
        }

        public PropertyInfo Member { get; }

        public Action<object,object> Setter { get; }

        public Func<object,object> Getter { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PropertyCompiled);
        }

        public bool Equals(PropertyCompiled other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Member == Member;
        }
        public override int GetHashCode()
        {
            return Member.GetHashCode();
        }
        protected virtual Func<object,object> BuildGetter()
        {
            if (Member.CanRead)
            {
                var par = Expression.Parameter(ObjectType);

                var body = Expression.Convert(Expression.Call(
                    Expression.Convert(par,Member.DeclaringType),
                    Member.GetMethod), ObjectType);
                return Expression.Lambda<Func<object,object>>(body,par).Compile();
            }
            return null;
        }
        protected virtual Action<object,object> BuildSetter()
        {
            if (Member.CanWrite)
            {
                var par1 = Expression.Parameter(ObjectType);
                var par2 = Expression.Parameter(ObjectType);

                var convPar1 = Expression.Convert(par1, Member.DeclaringType);
                var convPar2 = Expression.Convert(par2, Member.PropertyType);

                var body = Expression.Call(convPar1, Member.SetMethod,convPar2);

                return Expression.Lambda<Action<object,object>>(body,par1,par2).Compile();
            }
            return null;
        }
    }
}
