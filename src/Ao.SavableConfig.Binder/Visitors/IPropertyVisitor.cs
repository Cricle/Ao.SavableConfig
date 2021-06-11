using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Ao.SavableConfig.Binder.Visitors
{
    public interface IPropertyVisitor
    {
        void SetValue(object instance, object value, PropertyInfo info);

        object GetValue(object instance, PropertyInfo info);
    }
}
