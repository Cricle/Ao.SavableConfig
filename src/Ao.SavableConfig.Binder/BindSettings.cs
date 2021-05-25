using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Saver;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration
{
    public class BindSettings
    {
        public static readonly TimeSpan DefaultDelayTime = TimeSpan.FromMilliseconds(250);

        public BindSettings(object value, TimeSpan delayTime, IEnumerable<IChangeTransferCondition> conditions)
            :this(value,delayTime,conditions,null)
        {
        }
        public BindSettings(object value, TimeSpan delayTime, IEnumerable<IChangeTransferCondition> conditions, Action<Action> updater)
        {
            Value = value;
            DelayTime = delayTime;
            Conditions = conditions;
            Updater = updater;
        }

        public object Value { get; }

        public TimeSpan DelayTime { get; }

        public IEnumerable<IChangeTransferCondition> Conditions { get; }

        public Action<Action> Updater { get; }
    }
}
