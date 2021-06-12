using Ao.SavableConfig.Binder.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

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

            var properties = type.GetProperties();
            var map = new Dictionary<PropertyInfo, INameTransfer>();
            foreach (var item in properties)
            {
                var configPath = item.GetCustomAttribute<ConfigPathAttribute>();
                if (IsStepable(item)&&(force || CanStepIn(item)))
                {
                    var transfer = IdentityMapNameTransfer.FromTypeAttributes(configPath?.Name ?? item.Name, item.PropertyType, true);
                    map.Add(item, transfer);
                }
            }
            return map;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsStepable(PropertyInfo info)
        {
            var t = info.PropertyType;
            return t.IsClass && !t.IsAbstract && info.CanWrite && info.CanRead;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CanStepIn(PropertyInfo info)
        {
            return  info.GetCustomAttribute<ConfigStepInAttribute>() != null;
        }
    }
}
