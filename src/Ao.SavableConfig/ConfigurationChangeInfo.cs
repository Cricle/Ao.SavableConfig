using Microsoft.Extensions.Configuration;
namespace Ao.SavableConfig
{
    internal class ConfigurationChangeInfo : IConfigurationChangeInfo
    {
        public IConfiguration Sender { get; set; }

        public IConfigurationProvider Provider { get; set; }

        public string Key { get; set; }

        public string Old { get; set; }

        public string New { get; set; }

        public bool IsCreate { get; set; }
    }
}
