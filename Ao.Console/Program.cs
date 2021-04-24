using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Binder.Annotations;
using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Ao.Console
{
    [ConfigPath("Angular")]
    public class Root3
    {
        public virtual string Connection { get; set; }

        public virtual string Ip { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new SavableConfiurationBuilder();
            builder.AddJsonFile("a.json", false, true);
            var root = builder.Build();
            var inst = root.CreateProxy<Root3>();
            root.BindTwoWay(inst, JsonChangeTransferCondition.Instance);
            inst.Connection = "123";
            inst.Ip = "456";
            Task.Delay(1000).GetAwaiter().GetResult();
        }
    }
}
