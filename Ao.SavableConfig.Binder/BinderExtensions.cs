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

namespace Microsoft.Extensions.Configuration
{
    public static class BinderExtensions
    {
        public static dynamic CreateDynamic(this IConfiguration configuration)
        {
            return new DynamicConfiguration(configuration);
        }
        public static IDisposable BindTwoWay(this IConfigurationChangeNotifyable notifyable, object value,params IChangeTransferCondition[] changeTransferConditions)
        {
            var setting = new BindSettings(value, BindSettings.DefaultDelayTime, changeTransferConditions);
            return Bind(notifyable, setting, ConfigBindMode.TwoWay);
        }
        public static IDisposable Bind(this IConfigurationChangeNotifyable notifyable, BindSettings bindSettings, ConfigBindMode configBindMode)
        {
            var updater = bindSettings.Updater;
            if (updater is null)
            {
                updater = a => a();
            }
            var once = new ConcurrentOnce();
            var watcher = notifyable.CreateWatcher();
            if (configBindMode == ConfigBindMode.OneWay || configBindMode == ConfigBindMode.TwoWay)
            {
                notifyable.GetReloadToken()
                    .RegisterChangeCallback(Reload, null);
            }
            var notify = true;
            notifyable.Bind(bindSettings.Value);
            void Reload(object state)
            {
                notify = false;
                updater(() => notifyable.Bind(bindSettings.Value));
                notifyable.GetReloadToken()
                    .RegisterChangeCallback(Reload, null);
                notify = true;
            }
            async void handler(object o, IConfigurationChangeInfo e)
            {
                if (notify && await once.WaitAsync(bindSettings.DelayTime))
                {
                    watcher.ChangePushed -= handler;
                    try
                    {
                        var infos = watcher.ChangeInfos;
                        watcher.Clear();
                        var repo = ChangeReport.FromChanges(notifyable, infos);
                        var saver = new ChangeSaver(repo, bindSettings.Conditions);
                        var res = saver.EmitAndSave();
                        updater(() => notifyable.Bind(bindSettings.Value));
                    }
                    finally
                    {
                        watcher.ChangePushed += handler;
                    }
                }
            }
            if (configBindMode == ConfigBindMode.TwoWay)
            {
                watcher.ChangePushed += handler;
            }
            return new BindToken
            {
                Disposed = () =>
                {
                    watcher.ChangePushed -= handler;
                    watcher.Dispose();
                }
            };
        }
    }
}
