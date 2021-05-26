using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ao.SavableConfig
{
    /// <summary>
    /// 配置更改处理器
    /// </summary>
    /// <param name="info"></param>
    public delegate void ConfigurationChangedHandler(IConfigurationChangeInfo info);
    /// <summary>
    /// 可保存的配置根
    /// </summary>
    public class SavableConfigurationRoot : ConfigurationRoot, IConfigurationRoot, IConfiguration, IConfigurationChangeNotifyable
    {
        private readonly IList<IConfigurationProvider> _providers;
        /// <summary>
        /// Initializes a Configuration root with a list of providers.
        /// </summary>
        /// <param name="providers">The <see cref="IConfigurationProvider"/>s for this configuration.</param>
        public SavableConfigurationRoot(IList<IConfigurationProvider> providers)
            :base(providers)
        {
            this._providers = providers;
        }
        string IConfiguration.this[string key]
        {
            get => base[key];
            set => SetConfigValue(key, value);
        }
        private void SetConfigValue(string key,string value)
        {
            var anyOk = false;
            if (ConfigurationChanged is null)
            {
                base[key] = value;
            }
            else
            {
                var changeInfo = new ConfigurationChangeInfo
                {
                    Key = key,
                    New = value,
                    Sender = this,
                };
                for (int i = 0; i < _providers.Count; i++)
                {
                    var provider = _providers[i];
                    var ok = provider.TryGet(key, out var old);
                    if (ok)
                    {
                        provider.Set(key, value);
                        changeInfo.Old = old;
                        changeInfo.Provider = provider;
                        ConfigurationChanged(changeInfo);
                        anyOk = true;
                    }
                }
                if (!anyOk)
                {
                    var provider = _providers[_providers.Count - 1];
                    provider.Set(key, value);
                    changeInfo.Provider = provider;
                    changeInfo.IsCreate = true;
                    ConfigurationChanged(changeInfo);
                }
            }
        }
    
        /// <summary>
        /// Gets or sets the value corresponding to a configuration key.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        public new string this[string key]
        {
            get => base[key];
            set => SetConfigValue(key,value);
        }
        /// <inheritdoc/>
        public event ConfigurationChangedHandler ConfigurationChanged;
    }
}
