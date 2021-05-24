// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;

namespace Microsoft.Extensions.Configuration.Test
{
#if NET452
    internal static class ConfigurationExtensions
    {
        public static bool Exists(this IConfigurationSection section)
        {
            if (section == null)
            {
                return false;
            }
            return section.Value != null || section.GetChildren().Any();
        }

    }
#endif
    internal static class ConfigurationProviderExtensions
    {
        public static string Get(this IConfigurationProvider provider,string name)
        {
            if (provider.TryGet(name,out var val))
            {
                return val;
            }
            return null;
        }
        public static IConfigurationSection GetRequiredSection(this IConfiguration configuration, string key)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            IConfigurationSection section = configuration.GetSection(key);
            if (section.Exists())
            {
                return section;
            }

            throw new InvalidOperationException(key);
        }
    }
}
