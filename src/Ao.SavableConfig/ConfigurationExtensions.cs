using Microsoft.Extensions.Configuration;
using System;

namespace Ao.SavableConfig
{
    /// <summary>
    /// 对类型<see cref="IConfiguration"/>的扩展
    /// </summary>
    public static class ConfigurationExtensions
    {
        private static void ThrowCanNotCaseType(Type type)
        {
            throw new InvalidCastException($"Can't case {type} to IConfigurationChangeNotifyable");
        }
        /// <summary>
        /// 创建更改观察者
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ChangeWatcher CreateWatcher(this IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (configuration is IConfigurationChangeNotifyable notifyable)
            {
                return new ChangeWatcher(notifyable);
            }
            ThrowCanNotCaseType(configuration?.GetType());
            return null;
        }
        /// <summary>
        /// 创建空观察者
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static EmptyChangeWatcher CreateEmptyWatcher(this IConfigurationChangeNotifyable configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return new EmptyChangeWatcher(configuration);
        }
        /// <summary>
        /// 创建更改观察者
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static EmptyChangeWatcher CreateEmptyWatcher(this IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (configuration is IConfigurationChangeNotifyable notifyable)
            {
                return new EmptyChangeWatcher(notifyable);
            }
            ThrowCanNotCaseType(configuration?.GetType());
            return null;
        }
        /// <summary>
        /// 创建更改观察者
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ChangeWatcher CreateWatcher(this IConfigurationChangeNotifyable configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return new ChangeWatcher(configuration);
        }
    }
}
