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
    public class SavableConfigurationRoot : IConfigurationRoot, IConfigurationChangeNotifyable
    {
        private readonly IList<IConfigurationProvider> _providers;
        private readonly IList<IDisposable> _changeTokenRegistrations;
        private ConfigurationReloadToken _changeToken = new ConfigurationReloadToken();

        /// <summary>
        /// Initializes a Configuration root with a list of providers.
        /// </summary>
        /// <param name="providers">The <see cref="IConfigurationProvider"/>s for this configuration.</param>
        public SavableConfigurationRoot(IList<IConfigurationProvider> providers)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
            _changeTokenRegistrations = new List<IDisposable>(providers.Count);
            foreach (IConfigurationProvider p in providers)
            {
                p.Load();
                _changeTokenRegistrations.Add(ChangeToken.OnChange(() => p.GetReloadToken(), () => RaiseChanged()));
            }
        }

        /// <summary>
        /// The <see cref="IConfigurationProvider"/>s for this configuration.
        /// </summary>
        public IEnumerable<IConfigurationProvider> Providers => _providers;

        /// <summary>
        /// Gets or sets the value corresponding to a configuration key.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        public string this[string key]
        {
            get
            {
                for (int i = _providers.Count - 1; i >= 0; i--)
                {
                    IConfigurationProvider provider = _providers[i];

                    if (provider.TryGet(key, out string value))
                    {
                        return value;
                    }
                }

                return null;
            }
            set
            {
                if (_providers.Count == 0)
                {
                    throw new InvalidOperationException();
                }
                var changeInfo = new ConfigurationChangeInfo
                {
                    Key = key,
                    New = value,
                    Sender = this,
                };
                var anyOk = false;
                foreach (IConfigurationProvider provider in _providers)
                {
                    if (ConfigurationChanged is null)
                    {
                        provider.Set(key, value);
                    }
                    else
                    {
                        var ok = provider.TryGet(key, out var old);
                        if (ok)
                        {
                            provider.Set(key, value);
                            changeInfo.Old = old;
                            changeInfo.Provider = provider;
                            ConfigurationChanged.Invoke(changeInfo);
                            anyOk = true;
                        }
                    }
                }
                if(!anyOk)
                {
                    var provider = _providers[_providers.Count - 1];
                    provider.Set(key, value);
                    changeInfo.Provider = provider;
                    changeInfo.IsCreate = true;
                    ConfigurationChanged.Invoke(changeInfo);
                }
            }
        }
        /// <inheritdoc/>
        public event ConfigurationChangedHandler ConfigurationChanged;

        /// <summary>
        /// Gets the immediate children sub-sections.
        /// </summary>
        /// <returns>The children.</returns>
        public IEnumerable<IConfigurationSection> GetChildren() => this.GetChildrenImplementation(null);

        /// <summary>
        /// Returns a <see cref="IChangeToken"/> that can be used to observe when this configuration is reloaded.
        /// </summary>
        /// <returns>The <see cref="IChangeToken"/>.</returns>
        public IChangeToken GetReloadToken() => _changeToken;

        /// <summary>
        /// Gets a configuration sub-section with the specified key.
        /// </summary>
        /// <param name="key">The key of the configuration section.</param>
        /// <returns>The <see cref="IConfigurationSection"/>.</returns>
        /// <remarks>
        ///     This method will never return <c>null</c>. If no matching sub-section is found with the specified key,
        ///     an empty <see cref="IConfigurationSection"/> will be returned.
        /// </remarks>
        public IConfigurationSection GetSection(string key)
            => new SavableConfigurationSection(this, key);

        /// <summary>
        /// Force the configuration values to be reloaded from the underlying sources.
        /// </summary>
        public void Reload()
        {
            foreach (IConfigurationProvider provider in _providers)
            {
                provider.Load();
            }
            RaiseChanged();
        }

        private void RaiseChanged()
        {
            ConfigurationReloadToken previousToken = Interlocked.Exchange(ref _changeToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // dispose change token registrations
            foreach (IDisposable registration in _changeTokenRegistrations)
            {
                registration.Dispose();
            }

            // dispose providers
            foreach (IConfigurationProvider provider in _providers)
            {
                (provider as IDisposable)?.Dispose();
            }
        }
    }
}
