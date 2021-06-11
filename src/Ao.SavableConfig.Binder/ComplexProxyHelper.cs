using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ao.SavableConfig.Binder
{
    public class ComplexProxyHelper
    {
        public static readonly ComplexProxyHelper Default = new ComplexProxyHelper(ProxyHelper.Default);

        private readonly Dictionary<Type, ProxyCreator> creators;

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
        {
            return creators.ContainsKey(typeof(T));
        }
        public ProxyCreator GetCreatorOrDefault<T>()
        {
            if (creators.TryGetValue(typeof(T), out var creator))
            {
                return creator;
            }
            return null;
        }

        public T Build<T>(IConfiguration configuration)
        {
            var type = typeof(T);
            if (!creators.TryGetValue(type, out var creator))
            {
                creator = ProxyHelper.CreateComplexProxy<T>();
                creators.Add(type, creator);
            }
            return (T)creator.Build(configuration);
        }
    }
}
