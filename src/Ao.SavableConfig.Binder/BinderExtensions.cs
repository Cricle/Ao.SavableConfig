using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Saver;
using System;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    public static class BinderExtensions
    {
        public static dynamic CreateDynamic(this IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return new DynamicConfiguration(configuration);
        }
        public static BindBox BindTwoWay(this IConfigurationChangeNotifyable notifyable, object value, params IChangeTransferCondition[] changeTransferConditions)
        {
            if (notifyable is null)
            {
                throw new ArgumentNullException(nameof(notifyable));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (changeTransferConditions is null)
            {
                throw new ArgumentNullException(nameof(changeTransferConditions));
            }

            var setting = new BindSettings(value, BindSettings.DefaultDelayTime, changeTransferConditions);
            return Bind(notifyable, setting, ConfigBindMode.TwoWay);
        }
        public static ObservableBindBox BindNotifyTwoWay(this IConfigurationChangeNotifyable notifyable, INotifyPropertyChanged value, params IChangeTransferCondition[] changeTransferConditions)
        {
            if (notifyable is null)
            {
                throw new ArgumentNullException(nameof(notifyable));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (changeTransferConditions is null)
            {
                throw new ArgumentNullException(nameof(changeTransferConditions));
            }

            var setting = new BindSettings(value, BindSettings.DefaultDelayTime, changeTransferConditions);
            return BindNotifyNotify(notifyable, setting, ConfigBindMode.TwoWay);
        }
        public static ObservableBindBox BindNotifyNotify(this IConfigurationChangeNotifyable notifyable, BindSettings bindSettings, ConfigBindMode configBindMode)
        {
            if (notifyable is null)
            {
                throw new ArgumentNullException(nameof(notifyable));
            }

            if (bindSettings is null)
            {
                throw new ArgumentNullException(nameof(bindSettings));
            }

            var updater = bindSettings.Updater;
            if (updater is null)
            {
                updater = a => a();
            }
            var box = new ObservableBindBox(notifyable, bindSettings, configBindMode, updater);
            box.Bind();
            return box;
        }
        public static BindBox Bind(this IConfigurationChangeNotifyable notifyable, BindSettings bindSettings, ConfigBindMode configBindMode)
        {
            if (notifyable is null)
            {
                throw new ArgumentNullException(nameof(notifyable));
            }

            if (bindSettings is null)
            {
                throw new ArgumentNullException(nameof(bindSettings));
            }

            var updater = bindSettings.Updater;
            if (updater is null)
            {
                updater = a => a();
            }
            var box= new BindBox(notifyable, bindSettings, configBindMode, updater);
            box.Bind();
            return box;
        }
    }
}
