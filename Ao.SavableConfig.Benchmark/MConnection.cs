namespace Ao.SavableConfig.Benchmark
{
    public class MConnection
    {
        public virtual string ConnectionStr { get; set; }

        public virtual bool UseConnectionPool { get; set; }

        public virtual int? TimeOut { get; set; }
    }
}
