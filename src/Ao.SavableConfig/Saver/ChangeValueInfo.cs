using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Saver
{
    public class ChangeValueInfo
    {
        public ChangeValueInfo(
            IConfiguration configuration,
            IConfigurationChangeInfo info,
            ConfigurationTypes typeCode,
            bool isArray)
        {
            Configuration = configuration;
            Info = info;
            TypeCode = typeCode;
            IsArray = isArray;
        }

        public IConfiguration Configuration { get; }

        public IConfigurationChangeInfo Info { get; }

        public ConfigurationTypes TypeCode { get; }

        public bool IsArray { get; }
    }
}
