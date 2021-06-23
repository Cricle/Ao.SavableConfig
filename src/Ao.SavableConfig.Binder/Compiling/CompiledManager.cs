using System.Collections;
using System.Collections.Generic;

namespace Ao.SavableConfig.Binder
{
    public abstract class CompiledManager<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, ICompiledManager<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> compileds;

        public CompiledManager()
        {
            compileds = new Dictionary<TKey, TValue>();
        }

        public IEnumerable<TValue> Complateds => compileds.Values;

        public IEnumerable<TKey> Keys => compileds.Keys;

        public int Count => compileds.Count;

        public bool IsCompiled(TKey info)
        {
            return compileds.ContainsKey(info);
        }
        public TValue EnsureGetCompiled(TKey info)
        {
            var compiled = GetCompiled(info);
            if (compiled == null)
            {
                compiled = Compile(info);
                compileds.Add(info, compiled);
            }
            return compiled;
        }
        public TValue GetCompiled(TKey info)
        {
            compileds.TryGetValue(info, out var c);
            return c;
        }
        protected abstract TValue Compile(TKey key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return compileds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
