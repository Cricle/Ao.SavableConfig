using Ao.SavableConfig;
using Ao.SavableConfig.Saver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    public class BindBox : IDisposable
    {
        public IConfigurationChangeNotifyable ChangeNotifyable { get; }

        public BindSettings BindSettings { get; }

        public ConfigBindMode Mode { get; }

        public Action<Action> Updater { get; }

        public ChangeWatcher ChangeWatcher => changeWatcher;

        public bool IsBind => isBind;

        private ChangeWatcher changeWatcher;
        private readonly ConcurrentOnce once;

        private bool isBind;
        private bool notify = true;

        public event Action<BindBox, Exception> SaveException;
        public event Action<BindBox> Reloaded;
        public event Action<BindBox,IReadOnlyList<IConfigurationChangeInfo>> Saved;

        public BindBox(IConfigurationChangeNotifyable changeNotifyable, BindSettings bindSettings, ConfigBindMode mode, Action<Action> updater)
        {
            ChangeNotifyable = changeNotifyable ?? throw new ArgumentNullException(nameof(changeNotifyable));
            BindSettings = bindSettings ?? throw new ArgumentNullException(nameof(bindSettings));
            Mode = mode;
            Updater = updater ?? throw new ArgumentNullException(nameof(updater));
            once = new ConcurrentOnce();
        }

        public void Bind()
        {
            if (isBind)
            {
                return;
            }
            if (Mode != ConfigBindMode.OneTime)
            {
                changeWatcher = new ChangeWatcher(ChangeNotifyable);
            }
            if (Mode == ConfigBindMode.OneWay || Mode == ConfigBindMode.TwoWay)
            {
                ChangeNotifyable.GetReloadToken()
                    .RegisterChangeCallback(Reload, null);
            }
            if (Mode == ConfigBindMode.TwoWay || Mode == ConfigBindMode.OneWayToSource)
            {
                ChangeWatcher.ChangePushed += Handler;
            }
            ChangeNotifyable.Bind(BindSettings.Value);
            isBind = true;
        }
        public void UnBind()
        {
            if (!isBind)
            {
                return;
            }
            if (ChangeWatcher != null)
            {
                ChangeWatcher.ChangePushed -= Handler;
                ChangeWatcher.Dispose();
            }
            isBind = false;
        }

        private void Reload(object state)
        {
            notify = false;
            Updater(BindValue);
            ChangeNotifyable.GetReloadToken()
                .RegisterChangeCallback(Reload, null);
            notify = true;
            Reloaded?.Invoke(this);
        }

        private void Handler(object o, IConfigurationChangeInfo e)
        {
            _ = CoreHandler();
        }
        private void BindValue()
        {
            try
            {
                ChangeNotifyable.Bind(BindSettings.Value);
            }
            catch (Exception) { }
        }
        private async Task CoreHandler()
        {
            if (notify && await once.WaitAsync(BindSettings.DelayTime))
            {
                try
                {
                    var infos = ChangeWatcher.ChangeInfos;
                    ChangeWatcher.Clear();
                    var repo = ChangeReport.FromChanges(ChangeNotifyable, infos);
                    var saver = new ChangeSaver(repo, BindSettings.Conditions);
                    _= saver.EmitAndSave();
                    Updater(BindValue);
                    Saved?.Invoke(this, infos);
                }
                catch (Exception ex)
                {
                    SaveException?.Invoke(this, ex);
                }
            }
        }
        public void Dispose()
        {
            UnBind();
        }
    }
}
