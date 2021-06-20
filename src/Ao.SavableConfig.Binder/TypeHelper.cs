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
        public static object SafeChangeType(string value, Type type)
        {
            if (TryChangeType(value, type, out _, out var res))
            {
                return res;
            }
            return null;
        }
        public static bool TryChangeType(string value, Type type, out Exception ex, out object result)
        {
            ex = null;
            result = null;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                try
                {
                    result = TryChangeType(value, Nullable.GetUnderlyingType(type), out ex, out result);
                    return true;
                }
                catch (Exception e)
                {
                    ex = e;
                    return false;
                }
            }
            if (type.IsEnum)
            {
                try
                {
                    result = Enum.Parse(type, value);
                    return true;
                }
                catch (Exception e)
                {
                    ex = e;
                    return false;
                }
            }
            if (type.IsPrimitive || type == StringType || type == DecimalType)
            {
                try
                {
                    result = Convert.ChangeType(value, type);
                    return true;
                }
                catch (Exception e)
                {
                    ex = e;
                    return false;
                }
            }
            return false;
        }
        public static bool IsBaseType(Type type)
        {
            if (type.IsPrimitive)
            {
                return true;
            }
            if (type == StringType)
            {
                return true;
            }
            if (type == DecimalType)
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
