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
            return new DynamicConfiguration(configuration);
        }
        public static BindBox BindTwoWay(this IConfigurationChangeNotifyable notifyable, object value, params IChangeTransferCondition[] changeTransferConditions)
        {
            var setting = new BindSettings(value, BindSettings.DefaultDelayTime, changeTransferConditions);
            return Bind(notifyable, setting, ConfigBindMode.TwoWay);
        }
        public static BindBox Bind(this IConfigurationChangeNotifyable notifyable, BindSettings bindSettings, ConfigBindMode configBindMode)
        {
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
