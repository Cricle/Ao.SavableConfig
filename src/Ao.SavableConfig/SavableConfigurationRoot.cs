using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
            : base(providers)
        {
            _providers = providers;
        }
        string IConfiguration.this[string key]
        {
            get => base[key];
            set => SetConfigValue(key, value);
        }
        private void SetConfigValue(string key, string value)
        {
            if (ConfigurationChanged is null)
            {
                base[key] = value;
            }
            else
            {
                if (_providers.Count==0)
                {
                    throw new InvalidOperationException("No providers, can't do this");
                }
                var allNotSet = true;
                var providers = _providers;
                var count = providers.Count;
                string old = null;
                for (int i = 0; i < count; i++)
                {
                    var provider = providers[i];
                    var ok = provider.TryGet(key, out old);
                    if (ok)
                    {
                        provider.Set(key, value);
                        var changeInfo = new ConfigurationChangeInfo
                        {
                            Key = key,
                            New = value,
                            Sender = this,
                            Old = old,
                            Provider = provider
                        };
                        ConfigurationChanged(changeInfo);
                        allNotSet = false;
                    }
                }
                if (allNotSet)
                {
                    var provider = providers[providers.Count - 1];
                    provider.Set(key, value);

                    var changeInfo = new ConfigurationChangeInfo
                    {
                        Key = key,
                        New = value,
                        Sender = this,
                        IsCreate = true,
                        Provider = provider
                    };
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
            set => SetConfigValue(key, value);
        }
        /// <inheritdoc/>
        public event ConfigurationChangedHandler ConfigurationChanged;
#if NET452
        public new IConfigurationSection GetSection(string key)
        {
            return new SavableConfigurationSection(this, key);
        }
        IConfigurationSection IConfiguration.GetSection(string key)
        {
            return new SavableConfigurationSection(this, key);
        }
#endif
    }
#if NET452
    public class SavableConfigurationSection : ConfigurationSection, IConfigurationSection, IConfiguration
    {
        private readonly SavableConfigurationRoot root;
        public SavableConfigurationSection(SavableConfigurationRoot root, string path)
            : base(root, path)
        {
            this.root = root;
        }
        string IConfiguration.this[string key]
        {
            get => this[key];
            set => this[key] = value;
        }
        public new string Value
        {
            get => root[Path];
            set => root[Path] = value;
        }
        string IConfigurationSection.Value
        {
            get => root[Path];
            set => root[Path] = value;
        }

        public new string this[string key]
        {
            get => root[ConfigurationPath.Combine(Path, key)];
            set => root[ConfigurationPath.Combine(Path, key)] = value;
        }
        IConfigurationSection IConfiguration.GetSection(string key)
        {
            return root.GetSection(ConfigurationPath.Combine(Path, key));
        }
        public new IConfigurationSection GetSection(string key)
        {
            return root.GetSection(ConfigurationPath.Combine(Path, key));
        }
    }
#endif
}
