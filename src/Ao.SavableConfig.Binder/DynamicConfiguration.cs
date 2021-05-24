using System;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Configuration
{
    public class DynamicConfiguration : DynamicObject, INotifyPropertyChanged
    {
        private readonly IConfiguration configuration;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public DynamicConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var key = ConfigurationPath.Combine(indexes.Select(x => x.ToString()));
            result = configuration[key];
            RaisePropertyChanged(key);
            return true;
        }
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var key = ConfigurationPath.Combine(indexes.Select(x => x.ToString()));
            configuration[key] = value?.ToString();
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            configuration[binder.Name] = value?.ToString();
            if (configuration is IConfigurationSection section)
            {
                var path = ConfigurationPath.Combine(section.Path, binder.Name);
                RaisePropertyChanged(path);
            }
            else
            {
                RaisePropertyChanged(binder.Name);
            }
            return true;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new DynamicConfiguration(configuration.GetSection(binder.Name));
            return true;
        }
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            if (args.Length != 0 && args[0] is Type type)
            {
                result = configuration.Get(type);
                return true;
            }
            result = configuration.Get(binder.ReturnType);
            return true;
        }
    }
}
