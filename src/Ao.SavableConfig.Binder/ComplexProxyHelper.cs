using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Ao.SavableConfig.Binder
{
    public class ComplexProxyHelper
    {
        public static readonly ComplexProxyHelper Default = new ComplexProxyHelper(ProxyHelper.Default);

        private readonly Dictionary<Type, ProxyCreator> creators;

        public IReadOnlyDictionary<Type, ProxyCreator> Creators => creators;

        public ProxyHelper ProxyHelper { get; }

        public bool AutoAnalysis { get; }

        public ComplexProxyHelper(ProxyHelper proxyHelper)
            : this(proxyHelper, true)
        {
        }
        public ComplexProxyHelper(ProxyHelper proxyHelper, bool autoAnalysis)
        {
            AutoAnalysis = autoAnalysis;
            ProxyHelper = proxyHelper ?? throw new ArgumentNullException(nameof(proxyHelper));
            creators = new Dictionary<Type, ProxyCreator>();
        }

        public bool IsCreated<T>()
            where T : class
        {
            return IsCreated(typeof(T));
        }
        public bool IsCreated(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return creators.ContainsKey(type);
        }
        public ProxyCreator GetCreatorOrDefault(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (creators.TryGetValue(type, out var creator))
            {
                return creator;
            }
            return null;
        }
        public ProxyCreator GetCreatorOrDefault<T>()
            where T : class
        {
            return GetCreatorOrDefault(typeof(T));
        }

        public object Build(IConfiguration configuration, Type type)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (!creators.TryGetValue(type, out var creator))
            {
                creator = ProxyHelper.CreateComplexProxy(type);
                creators.Add(type, creator);
            }
            return creator.Build(configuration);
        }
        public T Build<T>(IConfiguration configuration)
            where T : class
        {
            return (T)Build(configuration, typeof(T));
        }
    }
}
