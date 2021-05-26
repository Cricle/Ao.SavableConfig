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
    [Obsolete("Please use ConfigurationExtensions.BuildSavable replace it, it will remove next version")]
    public class SavableConfiurationBuilder : ConfigurationBuilder,IConfigurationBuilder
    {
        IConfigurationRoot IConfigurationBuilder.Build()
        {
            return Build();
        }
        /// <inheritdoc cref="IConfigurationBuilder.Build"/>
        public new SavableConfigurationRoot Build()
        {
            return this.BuildSavable();
        }
    }
}
