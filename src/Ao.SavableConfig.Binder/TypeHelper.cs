using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Ao.SavableConfig.Binder
{
    internal static class TypeHelper
    {
        internal static readonly Type ObjectType = typeof(object);
        internal static readonly Type NullableType = typeof(Nullable<>);
        internal static readonly Type StringType = typeof(string);
        internal static readonly Type DecimalType = typeof(decimal);
        internal static readonly Type ByteArrayType = typeof(byte[]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullable(Type type)
        {
            return type.IsGenericType &&
                type.GetGenericTypeDefinition() == NullableType;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            if (IsConvertableType(type))
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
            if (IsNullable(type))
            {
                return TryChangeType(value, Nullable.GetUnderlyingType(type), out ex, out result);
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
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsConvertableType(Type type)
        {
            Debug.Assert(type != null);
            return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
        }
        public static bool IsBaseType(Type type)
        {
            if (IsConvertableType(type))
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
