using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig
{
    /// <summary>
    /// 可保持的配置生成器
    /// </summary>
    public class SavableConfiurationBuilder : IConfigurationBuilder
    {

#if NETSTANDARD1_0||NET452
        private readonly List<IConfigurationSource> sources = new List<IConfigurationSource>();
        private readonly Dictionary<string, object> properties=new Dictionary<string, object>();
        /// <summary>
        /// Gets a key/value collection that can be used to share data between the <see cref="IConfigurationBuilder"/>
        /// and the registered <see cref="IConfigurationProvider"/>s.
        /// </summary>
        public Dictionary<string, object> Properties => properties;
        /// <summary>
        /// Returns the sources used to obtain configuration values.
        /// </summary>
        public IEnumerable<IConfigurationSource> Sources => sources;

#else
        /// <summary>
        /// Returns the sources used to obtain configuration values.
        /// </summary>
        public IList<IConfigurationSource> Sources { get; } = new List<IConfigurationSource>();

        /// <summary>
        /// Gets a key/value collection that can be used to share data between the <see cref="IConfigurationBuilder"/>
        /// and the registered <see cref="IConfigurationProvider"/>s.
        /// </summary>
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
#endif

        /// <summary>
        /// Adds a new configuration source.
        /// </summary>
        /// <param name="source">The configuration source to add.</param>
        /// <returns>The same <see cref="IConfigurationBuilder"/>.</returns>
        public IConfigurationBuilder Add(IConfigurationSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
#if NETSTANDARD1_0||NET452
            sources.Add(source);
#else
            Sources.Add(source);
#endif
            return this;
        }

        IConfigurationRoot IConfigurationBuilder.Build()
        {
            return Build();
        }
        /// <summary>
        /// Builds an <see cref="IConfiguration"/> with keys and values from the set of providers registered in
        /// <see cref="Sources"/>.
        /// <para>And it is savable</para>
        /// </summary>
        /// <returns>An <see cref="IConfigurationRoot"/> with keys and values from the registered providers.</returns>
        public SavableConfigurationRoot Build()
        {
            var providers = new List<IConfigurationProvider>();
            foreach (IConfigurationSource source in Sources)
            {
                IConfigurationProvider provider = source.Build(this);
                providers.Add(provider);
            }
            return new SavableConfigurationRoot(providers);
        }
    }
}
