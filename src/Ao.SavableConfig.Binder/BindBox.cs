using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Binder.Visitors;
using Ao.SavableConfig.Saver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    public class BindBox : IDisposable
    {
        private static readonly Action Empty = () => { };

        public IConfigurationChangeNotifyable ChangeNotifyable { get; }

        public BindSettings BindSettings { get; }

        public ConfigBindMode Mode { get; }

        public Action<Action> Updater { get; }

        public INameTransfer NameTransfer { get; set; }

        public INamedCreator NamedCreator { get; set; }


        public ChangeWatcher ChangeWatcher => changeWatcher;

        public bool IsBind => isBind;

        private ObjectNamedCreator objectNamedCreator;
        private ChangeWatcher changeWatcher;
        private readonly ConcurrentOnce once;

        private bool isBind;
        private bool notify = true;

        public event Action<BindBox, Exception> SaveException;
        public event Action<BindBox> Reloaded;
        public event Action<BindBox, IReadOnlyList<IConfigurationChangeInfo>> Saved;

        public BindBox(IConfigurationChangeNotifyable changeNotifyable, BindSettings bindSettings, ConfigBindMode mode, Action<Action> updater)
        {
            ChangeNotifyable = changeNotifyable ?? throw new ArgumentNullException(nameof(changeNotifyable));
            BindSettings = bindSettings ?? throw new ArgumentNullException(nameof(bindSettings));
            Mode = mode;
            Updater = updater ?? throw new ArgumentNullException(nameof(updater));
            once = new ConcurrentOnce();
        }

        private void CreateNamedCreatorIfNeed()
        {
            if (objectNamedCreator == null)
            {
                var type = BindSettings.Value.GetType();
                objectNamedCreator = new ObjectNamedCreator(type,
                   NameTransfer ?? IdentityMapNameTransfer.FromTypeAttributes(type),
                   NamedCreator ?? IdentityNamedCreator.Instance,
                   CompilePropertyVisitor.Instance);
                objectNamedCreator.Analysis();
            }
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
                CreateNamedCreatorIfNeed();
            }
            if (Mode == ConfigBindMode.TwoWay || Mode == ConfigBindMode.OneWayToSource)
            {
                ChangeWatcher.ChangePushed += Handler;
                CreateNamedCreatorIfNeed();
            }
            Reload(null);
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
            if (notify)
            {
                notify = false;
                try
                {
                    Updater(Empty);
                    if (objectNamedCreator != null)
                    {
                        objectNamedCreator.Build(BindSettings.Value, ChangeNotifyable);
                    }
                    ChangeNotifyable.GetReloadToken()
                        .RegisterChangeCallback(Reload, null);
                    Reloaded?.Invoke(this);
                }
                finally
                {
                    notify = true;
                }
            }
        }

        private void Handler(object o, IConfigurationChangeInfo e)
        {
            _ = CoreHandler();
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
                    saver.EmitAndSave();
                    Updater(Empty);
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
