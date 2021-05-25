using System;

namespace Ao.SavableConfig.ConfigVisit
{
    public abstract class ConfigurationVisitor<TKeyPart>
        where TKeyPart: IConfigurationKeyPart
    {
        protected ConfigurationVisitor(string[] parts)
        {
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
            ValidateParts();
        }

        public string[] Parts { get; }

        protected virtual ConfigurationPartTypes GetPartType(string part)
        {
            if (string.IsNullOrEmpty(part))
            {
                throw new ArgumentException(nameof(part));
            }
#if NETSTANDARD1_3||NET452||NETSTANDARD1_1    
            var sp = part;
#else
            var sp = part.AsSpan();
#endif
            for (int i = 0; i < sp.Length; i++)
            {
                var p = sp[i];
                if (p < '0' || p > '9')
                {
                    return ConfigurationPartTypes.Named;
                }
            }
            return ConfigurationPartTypes.ArrayIndex;
        }
        protected abstract TKeyPart MakeKeyPart(int partIndex);

        protected virtual void ValidateParts()
        {
            for (int i = 0; i < Parts.Length; i++)
            {
                if (string.IsNullOrEmpty(Parts[i]))
                {
                    throw new ArgumentException($"The parts[{i}] is null or empty!");
                }
            }
        }
        public virtual bool VisitWrite()
        {
            for (int i = 0; i < Parts.Length; i++)
            {
                var kp = MakeKeyPart(i);
                var ok = VisitPart(kp);
                if (!ok)
                {
                    return ok;
                }
            }
            return true;
        }
        protected abstract bool VisitPart(TKeyPart keyPart);
    }
}
