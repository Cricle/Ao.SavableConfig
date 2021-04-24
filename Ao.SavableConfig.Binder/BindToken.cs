using System;

namespace Ao.SavableConfig.Binder
{
    internal class BindToken : IDisposable
    {
        public Action Disposed { get; set; }

        public void Dispose()
        {
            Disposed();
        }
    }
}
