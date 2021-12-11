// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Ao.ObjectDesign;
using Ao.SavableConfig.Binder.Resources;
using FastExpressionCompiler;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace Ao.SavableConfig.Binder
{
    public static class FastConfigurationBinder
    {
        private const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        /// <summary>
        /// Attempts to bind the configuration instance to a new instance of type T.
        /// If this configuration section has a value, that will be used.
        /// Otherwise binding by matching property names against configuration keys recursively.
        /// </summary>
        /// <typeparam name="T">The type of the new instance to bind.</typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <returns>The new instance of T if successful, default(T) otherwise.</returns>
        public static T FastGet<T>(this IConfiguration configuration)
            => FastGet<T>(configuration, null);

        /// <summary>
        /// Attempts to bind the configuration instance to a new instance of type T.
        /// If this configuration section has a value, that will be used.
        /// Otherwise binding by matching property names against configuration keys recursively.
        /// </summary>
        /// <typeparam name="T">The type of the new instance to bind.</typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="configureOptions">Configures the binder options.</param>
        /// <returns>The new instance of T if successful, default(T) otherwise.</returns>
        public static T FastGet<T>(this IConfiguration configuration, Action<FastBinderOptions> configureOptions)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            object result = FastGet(configuration,typeof(T), configureOptions);
            if (result == null)
            {
                return default(T);
            }
            return (T)result;
        }

        /// <summary>
        /// Attempts to bind the configuration instance to a new instance of type T.
        /// If this configuration section has a value, that will be used.
        /// Otherwise binding by matching property names against configuration keys recursively.
        /// </summary>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="type">The type of the new instance to bind.</param>
        /// <returns>The new instance if successful, null otherwise.</returns>
        public static object FastGet(this IConfiguration configuration, Type type)
            => FastGet(configuration,type, null);

        /// <summary>
        /// Attempts to bind the configuration instance to a new instance of type T.
        /// If this configuration section has a value, that will be used.
        /// Otherwise binding by matching property names against configuration keys recursively.
        /// </summary>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="type">The type of the new instance to bind.</param>
        /// <param name="configureOptions">Configures the binder options.</param>
        /// <returns>The new instance if successful, null otherwise.</returns>
        public static object FastGet(this IConfiguration configuration,
            Type type,
            Action<FastBinderOptions> configureOptions)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new FastBinderOptions();
            configureOptions?.Invoke(options);
            return BindInstance(type, instance: null, config: configuration, options: options);
        }

        /// <summary>
        /// Attempts to bind the given object instance to the configuration section specified by the key by matching property names against configuration keys recursively.
        /// </summary>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="key">The key of the configuration section to bind.</param>
        /// <param name="instance">The object to bind.</param>
        public static void FastBind(this IConfiguration configuration, string key, object instance)
            => FastBind(configuration.GetSection(key),instance);

        /// <summary>
        /// Attempts to bind the given object instance to configuration values by matching property names against configuration keys recursively.
        /// </summary>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="instance">The object to bind.</param>
        public static void FastBind(this IConfiguration configuration, object instance)
            => FastBind(configuration,instance, null);

        /// <summary>
        /// Attempts to bind the given object instance to configuration values by matching property names against configuration keys recursively.
        /// </summary>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="instance">The object to bind.</param>
        /// <param name="configureOptions">Configures the binder options.</param>
        public static void FastBind(this IConfiguration configuration, object instance, Action<FastBinderOptions> configureOptions)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (instance != null)
            {
                var options = new FastBinderOptions();
                configureOptions?.Invoke(options);
                BindInstance(instance.GetType(), instance, configuration, options);
            }
        }

        /// <summary>
        /// Extracts the value with the specified key and converts it to type T.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="key">The key of the configuration section's value to convert.</param>
        /// <returns>The converted value.</returns>
        public static T FastGetValue<T>(this IConfiguration configuration, string key)
        {
            return FastGetValue(configuration, key, default(T));
        }

        /// <summary>
        /// Extracts the value with the specified key and converts it to type T.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="key">The key of the configuration section's value to convert.</param>
        /// <param name="defaultValue">The default value to use if no value is found.</param>
        /// <returns>The converted value.</returns>
        public static T FastGetValue<T>(this IConfiguration configuration, string key, T defaultValue)
        {
            return (T)FastGetValue(configuration, typeof(T), key, defaultValue);
        }

        /// <summary>
        /// Extracts the value with the specified key and converts it to the specified type.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="type">The type to convert the value to.</param>
        /// <param name="key">The key of the configuration section's value to convert.</param>
        /// <returns>The converted value.</returns>
        public static object FastGetValue(
            this IConfiguration configuration,
            Type type,
            string key)
        {
            return FastGetValue(configuration, type, key, defaultValue: null);
        }

        /// <summary>
        /// Extracts the value with the specified key and converts it to the specified type.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="type">The type to convert the value to.</param>
        /// <param name="key">The key of the configuration section's value to convert.</param>
        /// <param name="defaultValue">The default value to use if no value is found.</param>
        /// <returns>The converted value.</returns>
        public static object FastGetValue(
            this IConfiguration configuration,
            Type type, string key,
            object defaultValue)
        {
            IConfigurationSection section = configuration.GetSection(key);
            string value = section.Value;
            if (value != null)
            {
                return ConvertValue(type, value, section.Path);
            }
            return defaultValue;
        }

        private static void BindNonScalar(this IConfiguration configuration, object instance, FastBinderOptions options)
        {
            if (instance != null)
            {
                List<PropertyInfo> modelProperties = GetAllProperties(instance.GetType());

                if (options.ErrorOnUnknownConfiguration)
                {
                    HashSet<string> propertyNames = new HashSet<string>(modelProperties.Select(mp => mp.Name),
                        StringComparer.OrdinalIgnoreCase);

                    IEnumerable<IConfigurationSection> configurationSections = configuration.GetChildren();
                    var missingPropertyNames = configurationSections
                        .Where(cs => !propertyNames.Contains(cs.Key))
                        .Select(mp => $"'{mp.Key}'");

                    if (missingPropertyNames.Any())
                    {
                        throw new InvalidOperationException(string.Format(Strings.Error_MissingConfig,
                            nameof(options.ErrorOnUnknownConfiguration), nameof(FastBinderOptions), instance.GetType(),
                            string.Join(", ", missingPropertyNames)));
                    }
                }

                foreach (PropertyInfo property in modelProperties)
                {
                    BindProperty(property, instance, configuration, options);
                }
            }
        }

        private static void BindProperty(PropertyInfo property, object instance, IConfiguration config, FastBinderOptions options)
        {
            // We don't support set only, non public, or indexer properties
            if (property.GetMethod == null ||
                (!options.BindNonPublicProperties && !property.GetMethod.IsPublic) ||
                property.GetMethod.GetParameters().Length > 0)
            {
                return;
            }

            bool hasSetter = property.SetMethod != null && (property.SetMethod.IsPublic || options.BindNonPublicProperties);

            if (!hasSetter)
            {
                // The property cannot be set so there is no point going further
                return;
            }

            object propertyValue = GetPropertyValue(property, instance, config, options);

            if (propertyValue != null)
            {
                AutoSetValue(property, instance, propertyValue);
            }
        }

        private static object BindToCollection(Type type, IConfiguration config, FastBinderOptions options)
        {
            var box = ListGenerator.Instance.EnsureCreate(type.GenericTypeArguments[0]);
            object instance = box.Newer();
            BindCollection(instance, box.GenType, config, options);
            return instance;
        }
        class GenBox
        {
            public Func<object> Newer;

            public Type GenType;
        }
        class StaticGenerator
        {
            public static readonly StaticGenerator Instance = new StaticGenerator();

            private readonly Dictionary<PropertyInfo, Action<object>> setters = new Dictionary<PropertyInfo, Action<object>>();
            private readonly Dictionary<PropertyInfo, Func<object>> getters = new Dictionary<PropertyInfo, Func<object>>();

            public Action<object> EnsureGetSetter(PropertyInfo prop)
            {
                if (!setters.TryGetValue(prop,out var f))
                {
                    f=CreateSetter(prop);
                    setters[prop]=f;
                }
                return f;
            }
            public Func<object> EnsureGetGetter(PropertyInfo prop)
            {
                if (!getters.TryGetValue(prop, out var f))
                {
                    f = CreateGetter(prop);
                    getters[prop] = f;
                }
                return f;
            }

            private Action<object> CreateSetter(PropertyInfo prop)
            {
                var par1 = Expression.Parameter(objectType);

                var body=Expression.Call(prop.SetMethod, Expression.Convert(par1,prop.PropertyType));
                return Expression.Lambda<Action<object>>(body, par1).CompileSys();
            }
            private Func<object> CreateGetter(PropertyInfo prop)
            {
                var body = Expression.Call(prop.GetMethod);
                return Expression.Lambda<Func<object>>(body).CompileSys();
            }
        }
        class NewerGenerator
        {
            public static readonly NewerGenerator Instance = new NewerGenerator();
            private readonly Dictionary<Type, TypeCreator> creators = new Dictionary<Type, TypeCreator>();

            public TypeCreator EnsureCreate(Type item)
            {
                if (!creators.TryGetValue(item ,out var f))
                {
                    f=Create(item);
                    creators[item] = f;
                    
                }
                return f;
            }

            private TypeCreator Create(Type item)
            {
                if (item.IsValueType)
                {
                    var body = Expression.Convert(Expression.New(item), objectType);
                    return Expression.Lambda<TypeCreator>(body).CompileSys();
                }
                return CompiledPropertyInfo.GetCreator(item);
            }
        }
        class ListGenerator
        {
            public static readonly ListGenerator Instance = new ListGenerator();
            private readonly Dictionary<Type, GenBox> creators = new Dictionary<Type, GenBox>();

            public GenBox EnsureCreate(Type item)
            {
                if (!creators.TryGetValue(item, out var f))
                {
                    f = Create(item);
                    creators[item] = f;
                }
                return f;
            }

            private GenBox Create(Type item)
            {
                var mapType = typeof(List<>).MakeGenericType(item);
                var createBody = Expression.New(mapType.GetConstructor(Type.EmptyTypes));
                var lam = Expression.Lambda<Func<object>>(createBody).CompileSys();
                var box = new GenBox
                {
                    GenType = mapType,
                    Newer = lam
                };
                return box;
            }
        }

        class MapGenerator
        {
            public static readonly MapGenerator Instance = new MapGenerator();
            private readonly Dictionary<(Type, Type), GenBox> creators = new Dictionary<(Type, Type), GenBox>();

            public GenBox EnsureCreate(Type key, Type value)
            {
                var k = (key, value);
                if (!creators.TryGetValue(k, out var f))
                {
                    f = Create(key, value);
                    creators[k] = f;
                }
                return f;
            }

            private GenBox Create(Type key, Type value)
            {
                var mapType = typeof(Dictionary<,>).MakeGenericType(key, value);
                var createBody = Expression.New(mapType.GetConstructor(Type.EmptyTypes));
                var lam = Expression.Lambda<Func<object>>(createBody).CompileSys();
                return new GenBox { Newer = lam, GenType = mapType };
            }
        }
        // Try to create an array/dictionary instance to back various collection interfaces
        private static object AttemptBindToCollectionInterfaces(

            Type type,
            IConfiguration config, FastBinderOptions options)
        {
            if (!type.IsInterface)
            {
                return null;
            }

            Type collectionInterface = FindOpenGenericInterface(typeof(IReadOnlyList<>), type);
            if (collectionInterface != null)
            {
                // IEnumerable<T> is guaranteed to have exactly one parameter
                return BindToCollection(type, config, options);
            }

            collectionInterface = FindOpenGenericInterface(typeof(IReadOnlyDictionary<,>), type);
            if (collectionInterface != null)
            {
                var box = MapGenerator.Instance.EnsureCreate(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
                object instance = box.Newer();
                BindDictionary(instance, box.GenType, config, options);
                return instance;
            }

            collectionInterface = FindOpenGenericInterface(typeof(IDictionary<,>), type);
            if (collectionInterface != null)
            {
                var box = MapGenerator.Instance.EnsureCreate(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
                object instance = box.Newer();
                BindDictionary(instance, collectionInterface, config, options);
                return instance;
            }

            collectionInterface = FindOpenGenericInterface(typeof(IReadOnlyCollection<>), type);
            if (collectionInterface != null)
            {
                // IReadOnlyCollection<T> is guaranteed to have exactly one parameter
                return BindToCollection(type, config, options);
            }

            collectionInterface = FindOpenGenericInterface(typeof(ICollection<>), type);
            if (collectionInterface != null)
            {
                // ICollection<T> is guaranteed to have exactly one parameter
                return BindToCollection(type, config, options);
            }

            collectionInterface = FindOpenGenericInterface(typeof(IEnumerable<>), type);
            if (collectionInterface != null)
            {
                // IEnumerable<T> is guaranteed to have exactly one parameter
                return BindToCollection(type, config, options);
            }

            return null;
        }

        private static object BindInstance(
            Type type,
            object instance, IConfiguration config, FastBinderOptions options)
        {
            // if binding IConfigurationSection, break early
            if (type == typeof(IConfigurationSection))
            {
                return config;
            }

            var section = config as IConfigurationSection;
            string configValue = section?.Value;
            if (configValue != null && TryConvertValue(type, configValue, section?.Path, out object convertedValue, out Exception error))
            {
                if (error != null)
                {
                    throw error;
                }

                // Leaf nodes are always reinitialized
                return convertedValue;
            }

            if (config != null && config.GetChildren().Any())
            {
                // If we don't have an instance, try to create one
                if (instance == null)
                {
                    // We are already done if binding to a new collection instance worked
                    instance = AttemptBindToCollectionInterfaces(type, config, options);
                    if (instance != null)
                    {
                        return instance;
                    }

                    instance = CreateInstance(type);
                }

                // See if its a Dictionary
                Type collectionInterface = FindOpenGenericInterface(typeof(IDictionary<,>), type);
                if (collectionInterface != null)
                {
                    BindDictionary(instance, collectionInterface, config, options);
                }
                else if (type.IsArray)
                {
                    instance = BindArray((Array)instance, config, options);
                }
                else
                {
                    // See if its an ICollection
                    collectionInterface = FindOpenGenericInterface(typeof(ICollection<>), type);
                    if (collectionInterface != null)
                    {
                        BindCollection(instance, collectionInterface, config, options);
                    }
                    // Something else
                    else
                    {
                        BindNonScalar(config, instance, options);
                    }
                }
            }

            return instance;
        }
        
        private static object CreateInstance(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
            {
                throw new InvalidOperationException(string.Format(Strings.Error_CannotActivateAbstractOrInterface, type));
            }

            if (type.IsArray)
            {
                if (type.GetArrayRank() > 1)
                {
                    throw new InvalidOperationException(string.Format(Strings.Error_UnsupportedMultidimensionalArray, type));
                }

                return Array.CreateInstance(type.GetElementType(), 0);
            }

            if (!type.IsValueType)
            {
                bool hasDefaultConstructor = type.GetConstructors(DeclaredOnlyLookup).Any(ctor => ctor.IsPublic && ctor.GetParameters().Length == 0);
                if (!hasDefaultConstructor)
                {
                    throw new InvalidOperationException(string.Format(Strings.Error_MissingParameterlessConstructor, type));
                }
            }

            try
            {
                
                return NewerGenerator.Instance.EnsureCreate(type)();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(Strings.Error_FailedToActivate, type), ex);
            }
        }

        private static void BindDictionary(
            object dictionary,

            Type dictionaryType,
            IConfiguration config, FastBinderOptions options)
        {
            // IDictionary<K,V> is guaranteed to have exactly two parameters
            Type keyType = dictionaryType.GenericTypeArguments[0];
            Type valueType = dictionaryType.GenericTypeArguments[1];
            bool keyTypeIsEnum = keyType.IsEnum;

            if (keyType != typeof(string) && !keyTypeIsEnum)
            {
                // We only support string and enum keys
                return;
            }

            PropertyInfo setter = dictionaryType.GetProperty("Item", DeclaredOnlyLookup);
            foreach (IConfigurationSection child in config.GetChildren())
            {
                object item = BindInstance(
                    type: valueType,
                    instance: null,
                    config: child,
                    options: options);
                if (item != null)
                {
                    if (keyType == typeof(string))
                    {
                        string key = child.Key;
                        setter.SetValue(dictionary, item, new object[] { key });
                    }
                    else if (keyTypeIsEnum)
                    {
                        object key = Enum.Parse(keyType, child.Key);
                        setter.SetValue(dictionary, item, new object[] { key });
                    }
                }
            }
        }

        private static void BindCollection(
            object collection,

            Type collectionType,
            IConfiguration config, FastBinderOptions options)
        {
            // ICollection<T> is guaranteed to have exactly one parameter
            Type itemType = collectionType.GenericTypeArguments[0];
            MethodInfo addMethod = collectionType.GetMethod("Add", DeclaredOnlyLookup);

            foreach (IConfigurationSection section in config.GetChildren())
            {
                try
                {
                    object item = BindInstance(
                        type: itemType,
                        instance: null,
                        config: section,
                        options: options);
                    if (item != null)
                    {
                        addMethod?.Invoke(collection, new[] { item });
                    }
                }
                catch
                {
                }
            }
        }

        private static Array BindArray(Array source, IConfiguration config, FastBinderOptions options)
        {
            IConfigurationSection[] children = config.GetChildren().ToArray();
            int arrayLength = source.Length;
            Type elementType = source.GetType().GetElementType();
            var newArray = Array.CreateInstance(elementType, arrayLength + children.Length);

            // binding to array has to preserve already initialized arrays with values
            if (arrayLength > 0)
            {
                Array.Copy(source, newArray, arrayLength);
            }

            for (int i = 0; i < children.Length; i++)
            {
                try
                {
                    object item = BindInstance(
                        type: elementType,
                        instance: null,
                        config: children[i],
                        options: options);
                    if (item != null)
                    {
                        newArray.SetValue(item, arrayLength + i);
                    }
                }
                catch
                {
                }
            }

            return newArray;
        }

        private static bool TryConvertValue(

            Type type,
            string value, string path, out object result, out Exception error)
        {
            error = null;
            result = null;
            if (type == objectType)
            {
                result = value;
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true;
                }
                return TryConvertValue(Nullable.GetUnderlyingType(type), value, path, out result, out error);
            }

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    result = converter.ConvertFromInvariantString(value);
                }
                catch (Exception ex)
                {
                    error = new InvalidOperationException(string.Format(Strings.Error_FailedBinding, path, type), ex);
                }
                return true;
            }

            if (type == typeof(byte[]))
            {
                try
                {
                    result = Convert.FromBase64String(value);
                }
                catch (FormatException ex)
                {
                    error = new InvalidOperationException(string.Format(Strings.Error_FailedBinding, path, type), ex);
                }
                return true;
            }

            return false;
        }

        private static object ConvertValue(
            Type type,
            string value, string path)
        {
            TryConvertValue(type, value, path, out object result, out Exception error);
            if (error != null)
            {
                throw error;
            }
            return result;
        }

        private static Type FindOpenGenericInterface(
            Type expected,

            Type actual)
        {
            if (actual.IsGenericType &&
                actual.GetGenericTypeDefinition() == expected)
            {
                return actual;
            }

            Type[] interfaces = actual.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == expected)
                {
                    return interfaceType;
                }
            }
            return null;
        }
        private static readonly Type objectType = typeof(object);
        private static List<PropertyInfo> GetAllProperties(Type type)
        {
            var allProperties = new List<PropertyInfo>();

            Type baseType = type;
            do
            {
                allProperties.AddRange(baseType.GetProperties(DeclaredOnlyLookup));
                baseType = baseType.BaseType;
            }
            while (baseType != objectType);

            return allProperties;
        }
        private static object AutoGetValue(PropertyInfo property,object instance)
        {
            object value;
            if (property.GetMethod.IsStatic)
            {
                var getter = StaticGenerator.Instance.EnsureGetGetter(property);
                value = getter();
            }
            else
            {
                var identity = new PropertyIdentity(property);
                var getter = CompiledPropertyInfo.GetGetter(identity);
                value = getter(instance);
            }
            return value;
        }

        private static void AutoSetValue(PropertyInfo property, object instance, object value)
        {
            if (property.DeclaringType.IsValueType)
            {
                property.SetValue(instance, value);
            }
            else
            {
                if (property.GetMethod.IsStatic)
                {
                    var getter = StaticGenerator.Instance.EnsureGetSetter(property);
                    getter(value);
                }
                else
                {
                    var identity = new PropertyIdentity(property);
                    var setter = CompiledPropertyInfo.GetSetter(identity);
                    setter(instance, value);
                }
            }
        }
        private static object GetPropertyValue(PropertyInfo property, object instance, IConfiguration config, FastBinderOptions options)
        {
            string propertyName = GetPropertyName(property);
            object value = AutoGetValue(property, instance);


            return BindInstance(
                property.PropertyType,
                value,
                config.GetSection(propertyName),
                options);
        }

        private static string GetPropertyName(MemberInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
#if USE_CONFIGURATION6_0

            // Check for a custom property name used for configuration key binding
            foreach (var attributeData in property.GetCustomAttributesData())
            {
                if (attributeData.AttributeType != typeof(ConfigurationKeyNameAttribute))
                {
                    continue;
                }

                // Ensure ConfigurationKeyName constructor signature matches expectations
                if (attributeData.ConstructorArguments.Count != 1)
                {
                    break;
                }

                // Assumes ConfigurationKeyName constructor first arg is the string key name
                string name = attributeData
                    .ConstructorArguments[0]
                    .Value?
                    .ToString();

                return !string.IsNullOrWhiteSpace(name) ? name : property.Name;
            }
#endif

            return property.Name;
        }
    }
}
