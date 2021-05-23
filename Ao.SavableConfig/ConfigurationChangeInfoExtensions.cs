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
            if (changeInfo is null)
            {
                throw new System.ArgumentNullException(nameof(changeInfo));
            }

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
            if (changeInfo is null)
            {
                throw new System.ArgumentNullException(nameof(changeInfo));
            }

            changeInfo.Provider.Set(changeInfo.Key, changeInfo.Old);
        }
    }
}