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
    public class Root4
    {
        [ConfigStepIn]
        [ConfigPath("root4")]
        public Root3 R3 { get; set; }
    }
    [ConfigPath("Angular")]
    public class Root3
    {
        public virtual string Connection { get; set; }

        public virtual string Ip { get; set; }

        public virtual int Port { get; set; }
        public virtual ulong Porta { get; set; }
        public virtual char Portb { get; set; }
        public virtual byte Portc { get; set; }
        public virtual long Portd { get; set; }
        public virtual short Porte { get; set; }
        public virtual byte Portf { get; set; }
        public virtual float Portg { get; set; }
        public virtual double Porth { get; set; }
        public virtual bool Portw { get; set; }

        public virtual object Portq { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new SavableConfiurationBuilder();
            builder.AddJsonFile("a.json", false, true);
            var root = builder.Build();
            var creator = ProxyHelper.Default.CreateComplexProxy<Root4>(true);
            var obj=(Root4)creator.Build(root);
            root.BindTwoWay(obj, JsonChangeTransferCondition.Instance);
            obj.R3.Porta = 231;
            System.Console.ReadLine();
        }
    }
}
