using Microsoft.Isam.Esent.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Wesent.Api.Providers
{
    public class SerializablePersistentDictionary<TKey, TValue>: IDictionary<TKey, TValue> 
        where TKey: IComparable<TKey>
    {
        static object _sync = new object();
        readonly IDictionary<TKey, TValue> _container;
        static  IDictionary<TKey, string> _persistent;
        readonly ISerializer<TValue> _serializer;

        public SerializablePersistentDictionary(IDictionary<TKey, string> persistent, ISerializer<TValue> serializer)
        {
            _serializer = serializer;
            _container = new Dictionary<TKey, TValue>();
            _persistent = persistent;
            _container = _persistent.ToDictionary( x=> x.Key, x=> _serializer.Deserialize(x.Value));
        }
        public void Add(TKey key, TValue value)
        {
            _container.Add(key, value);
            Task.Run( ()=> _persistent.Add(key, _serializer.Serialize(value)));
        }

        public bool ContainsKey(TKey key)
        {
            return _container.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _container.Keys; }
        }

        public bool Remove(TKey key)
        {
            return _container.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _container.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _container.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _container[key];
            }
            set
            {
                _container[key] = value;
                Task.Run(() =>
                {
                    _persistent[key] = _serializer.Serialize(value);
                });
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Task.Run(() =>
            {
                _persistent.Add(item.Key, _serializer.Serialize(item.Value));
            });
            _container.Add(item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _container.Contains(item); ;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _container.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _container.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            Task.Run(() => _persistent.Remove(item.Key));
            return _container.Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _container.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _container.GetEnumerator();
        }
    }
}