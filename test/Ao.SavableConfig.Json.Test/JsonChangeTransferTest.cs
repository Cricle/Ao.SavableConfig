using Ao.SavableConfig.Saver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.SavableConfig.Json.Test
{
    [TestClass]
    public class JsonChangeTransferTest
    {
        class SmallChangeInfo
        {
            public string Key { get; set; }

            public string Value { get; set; }
        }
        private static readonly object LeftObject = new
        {
            Title = "Hello",
            Student = new
            {
                Name = "jho",
                Age = 14
            },
            IncludeIds = new[] { 1, 2, 3, 4 }
        };
        private static readonly SmallChangeInfo ModifyProperty=new SmallChangeInfo 
        {
            Key="Title",
            Value="world"
        };
        private static readonly SmallChangeInfo AddProperty= new SmallChangeInfo
        {
            Key = "Room",
            Value = "44"
        };
        private static readonly SmallChangeInfo ModifyArray = new SmallChangeInfo
        {
            Key = "IncludeIds:2",
            Value = "88"
        };
        private static readonly SmallChangeInfo ModifyArrayToObject = new SmallChangeInfo
        {
            Key = "IncludeIds:2",
            Value = "88"
        };
    }
}
