using Microsoft.Extensions.Configuration;

namespace Ao.SavableConfig.Saver
{
    public partial class JsonChangeTransfer : IChangeTransfer
    {
        private static readonly string[] splitToken = new string[] { ConfigurationPath.KeyDelimiter };

        public bool IgnoreAdd { get; set; }
    }
}
