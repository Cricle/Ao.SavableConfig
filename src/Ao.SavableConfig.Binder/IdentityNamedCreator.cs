using Ao.SavableConfig.Binder.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class IdentityNamedCreator : INamedCreator
    {
        public static readonly IdentityNamedCreator Instance = new IdentityNamedCreator();

        public IReadOnlyDictionary<PropertyInfo, INameTransfer> Create(Type type, bool force)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var map = new Dictionary<PropertyInfo, INameTransfer>();
            var properties = type.GetProperties();
            foreach (var item in properties)
            {
                var t = item.PropertyType;
                var configPath = item.GetCustomAttribute<ConfigPathAttribute>();
                if (t.IsClass && !t.IsAbstract && item.CanRead && item.CanWrite
                    && force || item.GetCustomAttribute<ConfigStepInAttribute>() != null)
                {
                    var transfer = IdentityMapNameTransfer.FromTypeAttributes(configPath?.Name??item.Name, t, true);
                    map.Add(item, transfer);
                }
            }
            return map;
        }
    }
}
