using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig
{
    /// <summary>
    /// 配置更改信息
    /// </summary>
    public interface IConfigurationChangeInfo
    {
        /// <summary>
        /// 键
        /// </summary>
        string Key { get;}
        /// <summary>
        /// 新值
        /// </summary>
        string New { get; }
        /// <summary>
        /// 旧值
        /// </summary>
        string Old { get;}
        /// <summary>
        /// 修改的提供者
        /// </summary>
        IConfigurationProvider Provider { get; }
        /// <summary>
        /// 发起方
        /// </summary>
        IConfiguration Sender { get; }
        /// <summary>
        /// 是否是创建模式
        /// </summary>
        bool IsCreate { get; }
    }
}