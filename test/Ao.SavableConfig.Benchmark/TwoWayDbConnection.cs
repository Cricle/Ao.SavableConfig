using Ao.SavableConfig.Binder.Annotations;

namespace Ao.SavableConfig.Benchmark
{
    public class TwoWayDbConnection
    {
        [ConfigStepIn]
        public ConnectionInfo Mysql { get; set; }

        [ConfigStepIn]
        [ConfigPath("SqlService")]
        public ConnectionInfo Mssql { get; set; }
    }
}
