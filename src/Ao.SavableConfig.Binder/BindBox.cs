using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Binder.Visitors;
using Ao.SavableConfig.Saver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    public class BindBox : BindBoxBase
    {
        protected static readonly Action Empty = () => { };

        public Action<Action> Updater { get; }

        public ObjectNamedCreator ObjectNamedCreator => objectNamedCreator;

        private ObjectNamedCreator objectNamedCreator;

        public BindBox(IConfigurationChangeNotifyable changeNotifyable, BindSettings bindSettings, ConfigBindMode mode, Action<Action> updater)
            :base(changeNotifyable,bindSettings,mode)
        {
            Updater = updater ?? throw new ArgumentNullException(nameof(updater));
        }

        protected override void OnBind()
        {
            if (Mode != ConfigBindMode.OneTime)
            {
                var type = BindSettings.Value.GetType();
                objectNamedCreator = new ObjectNamedCreator(type,
                   NameTransfer ?? IdentityMapNameTransfer.FromTypeAttributes(type),
                   NamedCreator ?? IdentityNamedCreator.Instance,
                   CompilePropertyVisitor.Instance);
                objectNamedCreator.Analysis();

            }
        }
        protected override void OnReload(object state)
        {
            Updater(Empty);
            objectNamedCreator?.Build(BindSettings.Value, GetConfiguration());
        }
    }
}
