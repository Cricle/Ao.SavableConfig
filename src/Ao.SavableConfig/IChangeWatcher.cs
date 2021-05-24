using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Ao.SavableConfig
{
    /// <summary>
    /// 更改观察者
    /// </summary>
    public interface IChangeWatcher : IDisposable
    {
        /// <summary>
        /// 更改信息集合
        /// </summary>
        IReadOnlyList<IConfigurationChangeInfo> ChangeInfos { get; }
        /// <summary>
        /// 监听的更改配置目标
        /// </summary>
        IConfiguration Configuration { get; }
        /// <summary>
        /// 更改被推入时
        /// </summary>
        event EventHandler<IConfigurationChangeInfo> ChangePushed;
    }
}