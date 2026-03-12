using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWaveJS.NET
{
    /// <summary>
    /// Thread-safe Dictionary
    /// </summary>
    /// <remarks>
    /// ConcurrentDictionary wrapper class exposing a partial 
    /// "Dictionary-like" interface for use in this application.
    /// </remarks>
    internal class TSafeDictionary<K, V>
    {

        private ConcurrentDictionary<K, V> _dictionary = new ConcurrentDictionary<K, V>();

        public V this[K key]
        {
            get
            {
                if (!_dictionary.TryGetValue(key, out V value))
                {
                    throw new KeyNotFoundException();
                }
                return value;
            }
            private set { }
        }

        public ICollection<K> Keys
        {
            get { return (_dictionary.Keys); }
        }

        public void Add(K key, V callback)
        {
            if (!_dictionary.TryAdd(key, callback))
            {
                throw new ArgumentException();
            }
        }

        public bool Remove(K key)
        {
            return _dictionary.TryRemove(key, out _);
        }

        public bool ContainsKey(K key)
        {
            return _dictionary.ContainsKey(key);
        }
    }
}
