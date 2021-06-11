using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public interface INamedCreator
    {
        IReadOnlyDictionary<PropertyInfo, INameTransfer> Create(Type type, bool force);
    }
}
