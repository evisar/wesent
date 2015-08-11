using Microsoft.Isam.Esent.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using Wesent.Api.Configuration;

namespace Wesent.Api.Providers
{
    public class DefaultRepository<TKey, TValue> :IRepository<TKey, TValue>
        where TKey: IComparable<TKey>
    {
        readonly static IDictionary<TKey, TValue> _container;

        static DefaultRepository()
        {
            var path = Path.Combine(WesentConfiguration.DefaultInstance.Path, typeof(TValue).FullName);
            var persistent = new PersistentDictionary<TKey, string>(path);
            _container = new SerializablePersistentDictionary<TKey, TValue>(persistent, new JsonSerializer<TValue>());
        }
        public IQueryable<TValue> Query()
        {
            return _container.Values.AsQueryable();
        }

        public TValue Get(TKey key)
        {
            return _container[key];
        }

        public void Update(TValue value)
        {
            TKey key = GetKey(value);

            ((dynamic)value).Id = key;
            _container[key] = value;
        }

        public void Delete(TKey key)
        {
            _container.Remove(key);
        }

        protected internal virtual TKey GetKey(TValue value)
        {
            foreach (var pi in typeof(TValue).GetProperties())
            {
                if (pi.GetCustomAttributes(typeof(KeyAttribute), false) != null)
                {
                    return (TKey)(object)pi.GetValue(value);
                }
            }
            var key = default(TKey);
            var type = typeof(TKey);
            if (type == typeof(Guid))
            {
                key = (TKey)(dynamic)Guid.NewGuid();
            }
            else if (type == typeof(int) || type == typeof(int))
            {
                if (_container.Count == 0)
                {
                    key = (TKey)(dynamic)0;
                }
                else
                {
                    key = (TKey)((dynamic)_container.Keys.Max() + 1);
                }
            }
            else if (type == typeof(string))
            {
                key = (TKey)(dynamic)Guid.NewGuid().ToString();
            }
            return key;
        }
    }
}