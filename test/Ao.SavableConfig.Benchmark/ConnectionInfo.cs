namespace Ao.SavableConfig.Benchmark
{
    public class ConnectionInfo
    {
        //Must be virtual
        public virtual bool UseConnectionPool { get; set; }

        public virtual string Connection { get; set; }
    }
}
