using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Binder.Visitors;
using Ao.SavableConfig.Saver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    public abstract class BindBoxBase : IDisposable
    {
        public IConfigurationChangeNotifyable ChangeNotifyable { get; }

        public BindSettings BindSettings { get; }

        public ConfigBindMode Mode { get; }

        public INameTransfer NameTransfer { get; set; }

        public INamedCreator NamedCreator { get; set; }

        public bool IsBind => isBind;
        public ChangeWatcher ChangeWatcher => changeWatcher;

        private ChangeWatcher changeWatcher;
        private readonly ConcurrentOnce once;

        private bool isBind;
        private int notify = 1;

        public event Action<BindBoxBase, Exception> SaveException;
        public event Action<BindBoxBase> Reloaded;
        public event Action<BindBoxBase, IReadOnlyList<IConfigurationChangeInfo>> Saved;


        public BindBoxBase(IConfigurationChangeNotifyable changeNotifyable, BindSettings bindSettings, ConfigBindMode mode)
        {
            ChangeNotifyable = changeNotifyable ?? throw new ArgumentNullException(nameof(changeNotifyable));
            BindSettings = bindSettings ?? throw new ArgumentNullException(nameof(bindSettings));
            Mode = mode;
            once = new ConcurrentOnce();
        }
        protected virtual ObjectNamedCreator CreateNamedCreator()
        {
            var type = BindSettings.Value.GetType();
            var creator = new ObjectNamedCreator(type,
               NameTransfer ?? IdentityMapNameTransfer.FromTypeAttributes(type),
               NamedCreator ?? IdentityNamedCreator.Instance,
               CompilePropertyVisitor.Instance);
            creator.Analysis();
            return creator;
        }
        protected virtual IConfiguration GetConfiguration()
        {
            return ChangeNotifyable;
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
                GetConfiguration().GetReloadToken()
                    .RegisterChangeCallback(Reload, null);
            }
            if (Mode == ConfigBindMode.TwoWay || Mode == ConfigBindMode.OneWayToSource)
            {
                ChangeWatcher.ChangePushed += Handler;
            }
            OnBind();
            isBind = true;
            Reload(null);
        }
        protected virtual void OnBind()
        {

        }
        protected virtual void OnUnBind()
        {

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
            OnUnBind();
            isBind = false;
        }
        protected abstract void OnReload(object state);
        protected void Reload(object state)
        {
            OnReload(state);
            GetConfiguration().GetReloadToken()
                .RegisterChangeCallback(Reload, null);
            Reloaded?.Invoke(this);
        }

        private void Handler(object o, IConfigurationChangeInfo e)
        {
            _ = CoreHandler();
        }
        protected virtual void OnConfigChanged(IReadOnlyList<IConfigurationChangeInfo> changeInfos)
        {

        }
        private async Task CoreHandler()
        {
            var canEnter = await once.WaitAsync(BindSettings.DelayTime);
            if (canEnter && Interlocked.CompareExchange(ref notify, 0, 1) == 1)
            {
                try
                {
                    var infos = ChangeWatcher.ChangeInfos;
                    ChangeWatcher.Clear();
                    var repo = ChangeReport.FromChanges(GetConfiguration(), infos);
                    var saver = new ChangeSaver(repo, BindSettings.Conditions);
                    saver.EmitAndSave();
                    OnConfigChanged(infos);
                    Saved?.Invoke(this, infos);
                }
                catch (Exception ex)
                {
                    SaveException?.Invoke(this, ex);
                }
                finally
                {
                    Interlocked.Exchange(ref notify, 1);
                }
            }
        }
        public virtual void Dispose()
        {
            UnBind();
        }
    }
}
