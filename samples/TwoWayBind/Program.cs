using Ao.SavableConfig;
using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Binder;
using Microsoft.Extensions.Configuration;
using System;
using Ao.SavableConfig.Saver;
using System.Linq;

namespace TwoWayBind
{
    public class ConnectionInfo
    {
        //Must be virtual
        public virtual bool UseConnectionPool { get; set; }

        public virtual string Connection { get; set; }
    }
    public class DbConnection
    {
        [ConfigStepIn]
        public ConnectionInfo Mysql { get; set; }

        [ConfigStepIn]
        [ConfigPath("SqlService")]
        public ConnectionInfo Mssql { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new SavableConfiurationBuilder();
            builder.AddJsonFile("appsettings.json", false, true);
            var root = builder.Build();
            var connection = ProxyHelper.Default.CreateComplexProxy<DbConnection>(true);
            var value = (DbConnection)connection.Build(root.GetSection("DbConnections"));
            root.BindTwoWay(value, JsonChangeTransferCondition.Instance);
            while (true)
            {
                var str = Console.ReadLine();
                if (str == "p")
                {
                    Console.WriteLine("Mysql");
                    Console.WriteLine("Connection: \t" + value.Mysql.Connection);
                    Console.WriteLine("UseConnectionPool: \t" + value.Mysql.UseConnectionPool);
                    Console.WriteLine("Mssql");
                    Console.WriteLine("Connection: \t" + value.Mssql.Connection);
                    Console.WriteLine("UseConnectionPool: \t" + value.Mssql.UseConnectionPool);
                }
                else if (str=="w")
                {
                    var randon = new Random();
                    value.Mysql.Connection
                        = value.Mssql.Connection =
                        string.Concat(Enumerable.Range(0, 20)
                        .Select(x => randon.Next(0, 100).ToString())
                        .ToArray());
                }
            }
        }
    }
}
