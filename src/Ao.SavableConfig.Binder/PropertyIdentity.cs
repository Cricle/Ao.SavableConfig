using System;

namespace Ao.SavableConfig.Binder
{
    public class PropertyIdentity : IEquatable<PropertyIdentity>
    {
        public readonly Type Type;
        public readonly string PropertyName;

        public PropertyIdentity(Type type, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException($"“{nameof(propertyName)}”不能为 Null 或空。", nameof(propertyName));
            }

            Type = type ?? throw new ArgumentNullException(nameof(type));
            PropertyName = propertyName;
        }

        public override bool Equals(object obj)
        {
            if (obj is PropertyIdentity identity)
            {
                return Equals(identity);
            }
            return false;
        }
        public override int GetHashCode()
        {
#if NET452
            var h = 31 * 7 + Type.GetHashCode();
            h = h * 7 + PropertyName.GetHashCode();
            return h;
#else
            return HashCode.Combine(Type, PropertyName);
#endif
        }
        public override string ToString()
        {
            return $"{{{Type.FullName}, {PropertyName}}}";
        }

        public bool Equals(PropertyIdentity other)
        {
            if (other is null)
            {
                return false;
            }
            return other.Type == Type &&
                other.PropertyName == PropertyName;
        }
    }
}
