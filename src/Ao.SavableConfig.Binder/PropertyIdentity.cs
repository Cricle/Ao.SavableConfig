using System;
using System.Diagnostics;

namespace Ao.SavableConfig.Binder
{
    public readonly struct PropertyIdentity : IEquatable<PropertyIdentity>
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
            return Type.GetHashCode()+ PropertyName.GetHashCode();
        }
        public override string ToString()
        {
            return $"{{{Type.FullName}, {PropertyName}}}";
        }

        public bool Equals(PropertyIdentity other)
        {
            return other.Type == Type &&
                other.PropertyName == PropertyName;
        }
    }
}
