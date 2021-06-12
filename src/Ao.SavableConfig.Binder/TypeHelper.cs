using System;

namespace Ao.SavableConfig.Binder
{
    internal static class TypeHelper
    {
        internal static readonly Type ObjectType = typeof(object);
        internal static readonly Type NullableType = typeof(Nullable<>);
        internal static readonly Type StringType = typeof(string);
        internal static readonly Type DecimalType = typeof(decimal);
        internal static readonly Type ByteArrayType = typeof(byte[]);
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
            if (type==StringType)
            {
                return true;
            }
            if (type==DecimalType)
            {
                return true;
            }
            if (IsNullable(type))
            {
                return IsBaseType(Nullable.GetUnderlyingType(type));
            }
            return false;
        }
    }
}
