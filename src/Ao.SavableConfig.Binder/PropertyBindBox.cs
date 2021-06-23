using Ao.SavableConfig.Binder;
using System;
using System.Reflection;

namespace Microsoft.Extensions.Configuration
{
    public class PropertyBindBox
    {
        public PropertyBindBox(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            PropertyType = propertyInfo.PropertyType;
            IsBaseType = TypeHelper.IsBaseType(PropertyType);
        }

        public PropertyInfo PropertyInfo { get; }

        public Type PropertyType { get; }

        public bool IsBaseType { get; }

        public string Name { get; internal set; }

        public ObservableBindBox BindBox { get; internal set; }
    }
}
