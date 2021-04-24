using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig
{
    /// <summary>
    /// 配置更改可通知
    /// </summary>
    public interface IConfigurationChangeNotifyable : IConfiguration
    {
        /// <summary>
        /// 配置被更改了
        /// </summary>
        event ConfigurationChangedHandler ConfigurationChanged;
    }
}
