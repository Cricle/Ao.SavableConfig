using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Ao.SavableConfig
{
    /// <summary>
    /// 默认的更改观察者
    /// </summary>
    public class ChangeWatcher : IDisposable, IChangeWatcher
    {
        class ChangeIdentityComparer : IEqualityComparer<ChangeIdentity>
        {
            public static readonly ChangeIdentityComparer Instance = new ChangeIdentityComparer();
            public bool Equals(ChangeIdentity x, ChangeIdentity y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(ChangeIdentity obj)
            {
                return obj.GetHashCode();
            }
        }
        class ChangeIdentity: IEquatable<ChangeIdentity>
        {
            public readonly string Key;
            public readonly IConfigurationProvider Provder;

            public ChangeIdentity(string key, IConfigurationProvider provder)
            {
                Key = key;
                Provder = provder;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as ChangeIdentity);
            }

            public bool Equals(ChangeIdentity other)
            {
                if (other is null)
                {
                    return false;
                }
                return Key == other.Key &&
                    Provder == other.Provder;

            }

            public override int GetHashCode()
            {
                return Key?.GetHashCode() ?? 0 + Provder?.GetHashCode() ?? 0;
            }
        }
        private readonly IConfigurationChangeNotifyable watchConfiguration;

        private readonly Dictionary<ChangeIdentity, IConfigurationChangeInfo> changeInfos;

        /// <inheritdoc/>
        public IConfigurationChangeNotifyable Configuration => watchConfiguration;

        /// <inheritdoc/>
        public int ChangeCount => changeInfos.Count;

        /// <inheritdoc/>
        public IReadOnlyList<IConfigurationChangeInfo> ChangeInfos => changeInfos.Values.ToArray();

        IConfiguration IChangeWatcher.Configuration => Configuration;

        /// <inheritdoc/>
        public event EventHandler<IConfigurationChangeInfo> ChangePushed;

        /// <inheritdoc/>
        public event EventHandler ChangeCleared;
        /// <inheritdoc/>
        public event EventHandler ChangeMerged;

        public bool IgnoreSame { get; set; }
        /// <summary>
        /// 初始化类型<see cref="ChangeWatcher"/>
        /// </summary>
        /// <param name="notifyable"></param>
        public ChangeWatcher(IConfigurationChangeNotifyable notifyable)
        {
            changeInfos = new Dictionary<ChangeIdentity, IConfigurationChangeInfo>(ChangeIdentityComparer.Instance);
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
        /// <inheritdoc/>
        public void Merge()
        {
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
            if (!IgnoreSame && info.New == info.Old)
            {
                return;
            }
            if (Condition(info))
            {
                var tk = new ChangeIdentity(info.Key, info.Provider);
                changeInfos[tk] = info;
                ChangePushed?.Invoke(this, info);
            }
        }
    }
}
