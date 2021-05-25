using System;

namespace Ao.SavableConfig.ConfigVisit
{
    [Flags]
    public enum ConfigurationPartTypes : byte
    {
        Named = 0,
        ArrayIndex = 1
    }
}
