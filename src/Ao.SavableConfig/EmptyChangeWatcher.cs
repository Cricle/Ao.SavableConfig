using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Ao.SavableConfig
{
    /// <summary>
    /// 空的更改监听者
    /// </summary>
    public class EmptyChangeWatcher : IChangeWatcher
    {
        /// <summary>
        /// 初始化类型<see cref="EmptyChangeWatcher"/>
        /// </summary>
        /// <param name="configuration"></param>
        public EmptyChangeWatcher(IConfigurationChangeNotifyable configuration)
        {
            Configuration = notifyable = configuration ?? throw new ArgumentNullException(nameof(configuration));
            notifyable.ConfigurationChanged += OnConfigurationChanged;
        }

        private void OnConfigurationChanged(IConfigurationChangeInfo info)
        {
            ChangePushed?.Invoke(this, info);
        }

        private readonly IConfigurationChangeNotifyable notifyable;

        /// <inheritdoc/>
        public IReadOnlyList<IConfigurationChangeInfo> ChangeInfos => throw new NotSupportedException();
        /// <inheritdoc/>
        public IConfiguration Configuration { get; }

        /// <inheritdoc/>
        public event EventHandler<IConfigurationChangeInfo> ChangePushed;
        /// <inheritdoc/>
        public void Dispose()
        {
            notifyable.ConfigurationChanged -= OnConfigurationChanged;
        }
    }
}