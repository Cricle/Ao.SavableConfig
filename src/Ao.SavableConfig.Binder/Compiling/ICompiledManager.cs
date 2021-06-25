using System.Collections.Generic;

namespace Ao.SavableConfig.Binder
{
    public interface ICompiledManager<TKey, TValue>
    {
        int Count { get; }
        IEnumerable<TValue> Complateds { get; }
        IEnumerable<TKey> Keys { get; }

        TValue EnsureGetCompiled(TKey info);
        TValue GetCompiled(TKey info);
        bool IsCompiled(TKey info);
    }
}