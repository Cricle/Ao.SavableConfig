using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Binder.Visitors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.Configuration
{
    public class ObservableBindBox : BindBox
    {
        private static readonly string INotifyPropertyChangedTypeName = typeof(INotifyPropertyChanged).FullName;
        private readonly IPropertyVisitor propertyVisitor;
        private readonly Dictionary<string, PropertyBindBox> propertyConfigMap;
        private readonly Dictionary<string, PropertyBindBox> notifyPropertyMap;

        public IReadOnlyDictionary<string, PropertyBindBox> NotifyPropertyMap => notifyPropertyMap;

        public IReadOnlyDictionary<string, PropertyBindBox> PropertyConfigMap => propertyConfigMap;

        public INotifyPropertyChanged NotifyObject { get; }

        public IConfiguration Configuration { get; }

        public ObservableBindBoxUpdateModes UpdateMode { get; set; } = ObservableBindBoxUpdateModes.FindUpdate;

        public ObservableBindBox(IConfigurationChangeNotifyable changeNotifyable,
            BindSettings bindSettings,
            ConfigBindMode mode,
            Action<Action> updater)
            : this(changeNotifyable, changeNotifyable, bindSettings, mode, updater)
        {

        }
        public ObservableBindBox(IConfigurationChangeNotifyable changeNotifyable,
            IConfiguration configuration,
            BindSettings bindSettings,
            ConfigBindMode mode,
            Action<Action> updater)
            : base(changeNotifyable, bindSettings, mode, updater)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            NotifyObject = bindSettings.Value as INotifyPropertyChanged;
            if (NotifyObject is null)
            {
                throw new InvalidCastException($"Can't case {changeNotifyable.GetType().FullName} to {typeof(INotifyPropertyChanged).FullName}");
            }
            notifyPropertyMap = NotifyObject.GetType().GetProperties()
                .ToDictionary(x => x.Name, x => new PropertyBindBox(x));

            propertyConfigMap = new Dictionary<string, PropertyBindBox>();
            propertyVisitor = CompilePropertyVisitor.Instance;
        }
        protected override IConfiguration GetConfiguration()
        {
            return Configuration;
        }
        protected override void OnConfigChanged(IReadOnlyList<IConfigurationChangeInfo> changeInfos)
        {
            if (UpdateMode == ObservableBindBoxUpdateModes.FindUpdate)
            {
                foreach (var item in changeInfos)
                {
                    if (propertyConfigMap.TryGetValue(item.Key, out var box) && box.IsBaseType)
                    {
                        var val = TypeHelper.SafeChangeType(item.New, box.PropertyType);
                        propertyVisitor.SetValue(BindSettings.Value, val, box.PropertyInfo);
                    }
                }
            }
            else
            {
                Reload(null);
            }
        }
        protected virtual ObservableBindBox CreatePropertyBindBox(object instance, PropertyInfo propertyInfo)
        {
            var propertyBox = NotifyPropertyMap[propertyInfo.Name];
            var section = Configuration.GetSection(propertyBox.Name);
            var box = new ObservableBindBox(ChangeNotifyable, section,
                new BindSettings(instance, BindSettings.DelayTime, BindSettings.Conditions),
                Mode, Updater);
            box.UpdateMode = UpdateMode;
            return box;
        }

        protected override void OnBind()
        {
            base.OnBind();

            Debug.Assert(NotifyObject != null);
            propertyConfigMap.Clear();
            foreach (var x in NotifyPropertyMap.Values)
            {
                var transfedName = ObjectNamedCreator.NameTransfer.Transfer(NotifyObject, x.PropertyInfo.Name);

                var name = string.IsNullOrEmpty(transfedName) ? x.PropertyInfo.Name : transfedName;
                x.Name = name;
                Debug.Assert(!string.IsNullOrEmpty(name));

                if (Configuration is IConfigurationSection section)
                {
                    var targetName = ConfigurationPath.Combine(section.Path, name);
                    propertyConfigMap.Add(targetName, x);
                }
                else
                {
                    propertyConfigMap.Add(name, x);
                }

                if (x.PropertyInfo.PropertyType.GetInterface(INotifyPropertyChangedTypeName) != null)
                {
                    var val = propertyVisitor.GetValue(NotifyObject, x.PropertyInfo);
                    if (val != null)
                    {
                        if (x.BindBox != null)
                        {
                            x.BindBox.Dispose();
                        }
                        x.BindBox = CreatePropertyBindBox(val, x.PropertyInfo);
                        x.BindBox.Bind();
                    }
                }
            }
            NotifyObject.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var prop = NotifyPropertyMap[e.PropertyName];
            var val = propertyVisitor.GetValue(NotifyObject, prop.PropertyInfo);
            if (val is INotifyPropertyChanged)
            {
                if (prop.BindBox != null)
                {
                    prop.BindBox.Dispose();
                }
                prop.BindBox = CreatePropertyBindBox(val, prop.PropertyInfo);
                prop.BindBox.Bind();
            }
            else
            {
                Configuration[prop.Name] = val?.ToString();
            }
        }

        protected override void OnUnBind()
        {
            base.OnUnBind();
            Debug.Assert(NotifyObject != null);
            foreach (var item in NotifyPropertyMap.Values)
            {
                if (item.BindBox != null)
                {
                    item.BindBox.UnBind();
                }
            }
            NotifyObject.PropertyChanged -= OnPropertyChanged;
        }
        public override void Dispose()
        {
            base.Dispose();
            foreach (var item in NotifyPropertyMap.Values)
            {
                item.BindBox?.Dispose();
            }
            notifyPropertyMap.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
