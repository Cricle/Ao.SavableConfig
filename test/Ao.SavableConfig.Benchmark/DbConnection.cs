using Ao.SavableConfig.Binder.Annotations;

namespace Ao.SavableConfig.Benchmark
{
    public class DbConnection
    {
        [ConfigStepIn]
        public Connection Mysql { get; set; }

        [ConfigStepIn]
        public Connection MsSql { get; set; }
    }
}
