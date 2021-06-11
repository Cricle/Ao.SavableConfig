using Ao.SavableConfig;
using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Binder;
using Microsoft.Extensions.Configuration;
using System;
using Ao.SavableConfig.Saver;
using System.Linq;
using System.Diagnostics;

namespace TwoWayBind
{
    public class ConnectionInfo
    {
        //Must be virtual
        public virtual bool UseConnectionPool { get; set; }

        public virtual string Connection { get; set; }

        [ConfigStepIn]
        public virtual DbOptions Options { get; set; }
    }
    public class DbOptions
    {
        public virtual bool UsingLinq { get; set; }

        public virtual int TimeOut { get; set; }

        public DbConnection Connection { get; set; }
    }
    [ConfigStepIn]
    [ConfigPath("DbConnections")]
    public class DbConnection
    {
        public ConnectionInfo Mysql { get; set; }

        [ConfigPath("SqlService")]
        public ConnectionInfo Mssql { get; set; }

        [ConfigPath(Absolute = true)]
        public Logging Loggings { get; set; }
    }
    public class Logging
    {
        public virtual string Default { get; set; }

        public virtual string Quto { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", false, true);
            builder.AddJsonFile("appsettings2.json", false, true);
            var root = builder.BuildSavable();
            var value = root.AutoCreateProxy<DbConnection>();
            var box=root.BindTwoWay(value, JsonChangeTransferCondition.Instance);
            while (true)
            {
                var str = Console.ReadLine();
                if (str == "p")
                {
                    Console.WriteLine("Mysql");
                    Console.WriteLine("Connection: \t" + value.Mysql.Connection);
                    Console.WriteLine("UseConnectionPool: \t" + value.Mysql.UseConnectionPool);
                    Console.WriteLine("UseConnectionPool.UsingLinq: \t\t" + value.Mysql.Options.UsingLinq);
                    Console.WriteLine("UseConnectionPool.TimeOut: \t\t" + value.Mysql.Options.TimeOut);
                    Console.WriteLine("Mssql");
                    Console.WriteLine("Connection: \t" + value.Mssql.Connection);
                    Console.WriteLine("UseConnectionPool: \t" + value.Mssql.UseConnectionPool);
                    Console.WriteLine("UseConnectionPool.UsingLinq: \t\t" + value.Mssql.Options.UsingLinq);
                    Console.WriteLine("UseConnectionPool.TimeOut: \t\t" + value.Mssql.Options.TimeOut);
                    Console.WriteLine("Logging");
                    Console.WriteLine("Default: \t" + value.Loggings.Default);
                    Console.WriteLine("Quto: \t" + value.Loggings.Quto);
                }
                else if (str == "w")
                {
                    var randon = new Random();
                    value.Mysql.Connection
                        = value.Mssql.Connection
                        = value.Loggings.Default =
                        value.Loggings.Quto =
                        string.Concat(Enumerable.Range(0, 20)
                        .Select(x => randon.Next(0, 100).ToString())
                        .ToArray());

                    value.Mysql.Options.TimeOut = randon.Next(0, 99999);
                    value.Mssql.Options.TimeOut = randon.Next(0, 99999);
                }
            }
        }
    }
}
