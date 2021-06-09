using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ao.SavableConfig
{
    public class ConcurrentOnce
    {
        private object token = new object();
        public object Token => token;

        public bool Exchange()
        {
            return ExchangeCore(token);
        }
#if !NETSTANDARD1_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool ExchangeCore(object tk)
        {
            return Interlocked.CompareExchange(ref token, new object(), tk) == tk;
        }

        public async Task<bool> WaitAsync(TimeSpan delayTime)
        {
            var tk = token;
            await Task.Delay(delayTime).ConfigureAwait(false);
            return ExchangeCore(tk);
        }

        public override string ToString()
        {
            return $"{{{Token}}}";
        }
    }
}
