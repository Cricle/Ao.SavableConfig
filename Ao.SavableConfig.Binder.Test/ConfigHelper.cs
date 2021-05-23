using Microsoft.Extensions.Configuration;
namespace Ao.SavableConfig.Binder.Test
{
    internal static class ConfigHelper
    {
        public static SavableConfigurationRoot CreateEmptyRoot()
        {
            var builder = new SavableConfiurationBuilder();
            builder.AddInMemoryCollection();
            return builder.Build();
        }
    }
}
