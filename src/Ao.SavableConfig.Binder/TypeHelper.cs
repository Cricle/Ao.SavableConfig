using System;

namespace Ao.SavableConfig.Binder
{
    internal static class TypeHelper
    {
        internal static readonly Type NullableType = typeof(Nullable<>);
        internal static readonly Type StringType = typeof(string);
        internal static readonly Type DecimalType = typeof(decimal);
        public static bool IsNullable(Type type)
        {
            return type.IsGenericType &&
                type.GetGenericTypeDefinition() == NullableType;
        }
        public static bool IsBaseType(Type type)
        {
            if (type.IsPrimitive)
            {
                return true;
            }
            if (type.IsEquivalentTo(StringType))
            {
                return true;
            }
            if (type.IsEquivalentTo(DecimalType))
            {
                return true;
            }
            if (IsNullable(type))
            {
                return IsBaseType(type.GenericTypeArguments[0]);
            }
            return false;
        }
    }
}
