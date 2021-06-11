using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ao.SavableConfig.Saver
{
    /// <summary>
    /// 更改报告
    /// </summary>
    public class ChangeReport
    {
        private static readonly char[] numberChars = "0123456789".ToCharArray();
        /// <summary>
        /// 初始化类型<see cref="ChangeReport"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="provider"></param>
        /// <param name="includeChangeInfo"></param>
        public ChangeReport(
            IConfiguration configuration,
            IConfigurationProvider provider,
            IReadOnlyList<IConfigurationChangeInfo> includeChangeInfo)
        {
            Configuration = configuration;
            Provider = provider;
#if NETSTANDARD1_1 || NET452
            IncludeChangeInfo = includeChangeInfo ?? new IConfigurationChangeInfo[0];
#else
            IncludeChangeInfo = includeChangeInfo ?? Array.Empty<IConfigurationChangeInfo>();
#endif
            if (includeChangeInfo != null)
            {
                for (int i = 0; i < includeChangeInfo.Count; i++)
                {
                    if (includeChangeInfo[i].Provider != provider)
                    {
                        throw new ArgumentException($"The {i} element provider is not equal {Provider}");
                    }
                }
            }
        }
        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 配置提供器
        /// </summary>
        public IConfigurationProvider Provider { get; }
        /// <summary>
        /// 包含的更改信息
        /// </summary>
        public IReadOnlyList<IConfigurationChangeInfo> IncludeChangeInfo { get; }
        /// <summary>
        /// 获取更改值报告
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, ChangeValueInfo> GetValueReport()
        {
            var map = new Dictionary<string, ChangeValueInfo>(IncludeChangeInfo.Count);
            foreach (var item in IncludeChangeInfo)
            {
                var isArray = item.Key.TrimEnd(numberChars).EndsWith(ConfigurationPath.KeyDelimiter);
                if (isArray)
                {
                    map[item.Key]= new ChangeValueInfo(Configuration,item,  ConfigurationTypes.Array,true);
                }
                else
                {
                    var type = TypeHelper.GetTypeCode(item.New);
                    map[item.Key]= new ChangeValueInfo(Configuration, item, type, false);
                }
            }
            return map;
        }
        /// <summary>
        /// 从更改列表超级更改报告
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static IEnumerable<ChangeReport> FromChanges(IConfiguration configuration,IReadOnlyList<IConfigurationChangeInfo> datas)
        {
            foreach (var item in datas.GroupBy(x=>x.Provider))
            {
                yield return new ChangeReport(configuration, item.Key, item.ToArray());
            }
        }
    }
}
