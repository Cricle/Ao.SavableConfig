using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class NullNamedCreator : INamedCreator
    {
        public static readonly NullNamedCreator Instance = new NullNamedCreator();

        public IReadOnlyDictionary<PropertyInfo, INameTransfer> Create(Type type, bool force)
        {
            return new Dictionary<PropertyInfo, INameTransfer>(0);
        }
    }
}
