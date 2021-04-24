using System;

namespace Ao.SavableConfig.Saver
{
    [Flags]
    public enum ConfigurationTypes
    {
        Null,
        Boolean,
        Number,
        Single,
        String,
        Object,
        Array
    }
}
