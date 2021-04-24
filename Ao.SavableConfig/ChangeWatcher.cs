using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace Ao.SavableConfig
{
    /// <summary>
    /// 默认的更改观察者
    /// </summary>
    public class ChangeWatcher : IDisposable, IChangeWatcher
    {
        private readonly IConfigurationChangeNotifyable watchConfiguration;

        private Stack<IConfigurationChangeInfo> changeInfos;

        /// <inheritdoc/>
        public IConfigurationChangeNotifyable Configuration => watchConfiguration;

        /// <inheritdoc/>
        public int ChangeCount => changeInfos.Count;

        /// <inheritdoc/>
        public IReadOnlyList<IConfigurationChangeInfo> ChangeInfos => changeInfos.ToArray();

        IConfiguration IChangeWatcher.Configuration => Configuration;

        /// <inheritdoc/>
        public event EventHandler<IConfigurationChangeInfo> ChangePushed;

        /// <inheritdoc/>
        public event EventHandler ChangeCleared;
        /// <inheritdoc/>
        public event EventHandler ChangeMerged;
        /// <summary>
        /// 初始化类型<see cref="ChangeWatcher"/>
        /// </summary>
        /// <param name="notifyable"></param>
        public ChangeWatcher(IConfigurationChangeNotifyable notifyable)
        {
            changeInfos = new Stack<IConfigurationChangeInfo>();
            watchConfiguration = notifyable ?? throw new ArgumentNullException(nameof(notifyable));
            notifyable.ConfigurationChanged += Notifyable_ConfigurationChanged;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            watchConfiguration.ConfigurationChanged -= Notifyable_ConfigurationChanged;
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            Stop();
        }
        /// <inheritdoc/>
        public void Clear()
        {
            changeInfos.Clear();
            ChangeCleared?.Invoke(this, EventArgs.Empty);
        }
        internal readonly struct ConfigurationPair : IEquatable<ConfigurationPair>
        {
            private readonly IConfiguration Configuration;
            private readonly IConfigurationProvider ConfigurationProvider;

            public ConfigurationPair(IConfiguration configuration, IConfigurationProvider configurationProvider)
            {
                Configuration = configuration;
                ConfigurationProvider = configurationProvider;
            }

            public override int GetHashCode()
            {
                var hash = 0;
                if (Configuration != null)
                {
                    hash = Configuration.GetHashCode();
                }
                if (ConfigurationProvider != null)
                {
                    hash |= ConfigurationProvider.GetHashCode();
                }
                return hash;
            }
            public override bool Equals(object obj)
            {
                if (obj is ConfigurationPair e)
                {
                    return Equals(e);
                }
                return false;
            }
            public bool Equals(ConfigurationPair other)
            {
                return other.Configuration == Configuration &&
                    other.ConfigurationProvider == ConfigurationProvider;
            }
        }
        /// <inheritdoc/>
        public void Merge()
        {
            var map = new Dictionary<ConfigurationPair, IConfigurationChangeInfo>();
            foreach (var item in changeInfos)
            {
                var pari = new ConfigurationPair(item.Sender, item.Provider);
                if (!map.ContainsKey(pari))
                {
                    map[pari] = item;
                }
            }
            changeInfos = new Stack<IConfigurationChangeInfo>(map.Values);
            ChangeMerged?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 调用以确保更改是否可以增加
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected virtual bool Condition(IConfigurationChangeInfo info)
        {
            return true;
        }
        private void Notifyable_ConfigurationChanged(IConfigurationChangeInfo info)
        {
            if (Condition(info))
            {
                changeInfos.Push(info);
                ChangePushed?.Invoke(this, info);
            }
        }
    }
}
