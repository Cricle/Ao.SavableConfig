using System;

namespace Ao.SavableConfig.Saver
{
    internal static class TypeHelper
    {
        public static ConfigurationTypes GetTypeCode(string value)
        {
            if (value is null)
            {
                return ConfigurationTypes.Null;
            }
            if (value ==string.Empty)
            {
                return ConfigurationTypes.String;
            }
            if (string.Equals(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, bool.FalseString, StringComparison.OrdinalIgnoreCase))
            {
                return ConfigurationTypes.Boolean;
            }
            if (long.TryParse(value,out _))
            {
                return ConfigurationTypes.Number;
            }
            if (double.TryParse(value ,out _))
            {
                return ConfigurationTypes.Single;
            }
            return ConfigurationTypes.String;
        }
    }
}
