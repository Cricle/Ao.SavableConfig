using System;

namespace Ao.SavableConfig.Saver
{
    [Flags]
    public enum ConfigurationTypes
    {
        Null = 0,
        Boolean = 1,
        Number = 2,
        Single = 3,
        String = 4,
        Object = 5,
        Array = 6
    }
}
