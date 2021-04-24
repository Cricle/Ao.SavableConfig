using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig
{
    /// <summary>
    /// 对类型<see cref="IConfigurationChangeInfo"/>的扩展
    /// </summary>
    public static class ConfigurationChangeInfoExtensions
    {
        /// <summary>
        /// 创建撤销更改信息
        /// </summary>
        /// <param name="changeInfo"></param>
        /// <returns></returns>
        public static IConfigurationChangeInfo CreateUndo(this IConfigurationChangeInfo changeInfo)
        {
            return new ConfigurationChangeInfo
            {
                Key = changeInfo.Key,
                New = changeInfo.Old,
                Old = changeInfo.New,
                Provider = changeInfo.Provider,
                Sender = changeInfo.Sender
            };
        }
        /// <summary>
        /// 撤销操作
        /// </summary>
        /// <param name="changeInfo"></param>
        public static void Undo(this IConfigurationChangeInfo changeInfo)
        {
            changeInfo.Provider.Set(changeInfo.Key, changeInfo.Old);
        }
    }
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
    }
}