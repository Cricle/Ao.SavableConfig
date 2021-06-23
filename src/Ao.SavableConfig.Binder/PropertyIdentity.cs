using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
            return Type.GetHashCode()+ PropertyName.GetHashCode();
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
