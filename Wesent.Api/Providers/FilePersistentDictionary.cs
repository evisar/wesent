using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Wesent.Api.Providers
{
    public class FilePersistentDictionary<TKey, TValue>: IRepository<TKey,TValue>
        where TKey: IComparable<TKey>
    {

        readonly string _path;
        readonly ISerializer<TValue> _serializer;

        public FilePersistentDictionary(string path, ISerializer<TValue> serializer)
        {
            _path = path;
        }



        public void Add(TKey key, string value)
        {
            var path = Path.Combine(_path, key.ToString());
            File.WriteAllText(path, value);
        }

        public bool ContainsKey(TKey key)
        {
            var path = Path.Combine(_path, key.ToString());
            return File.Exists(path);
        }

        public ICollection<TKey> Keys
        {
            get {
                return (from string file in Directory.GetFiles(_path)
                        let name = new FileInfo(file).Name
                        select (TKey)Convert.ChangeType(name, typeof(TKey))).ToList();
            }
        }

        public bool Remove(TKey key)
        {
            var path = Path.Combine(_path, key.ToString());
            File.Delete(path);
            return true;
        }

        public bool TryGetValue(TKey key, out string value)
        {
            value = null;
            var path = Path.Combine(_path, key.ToString());
            if (!File.Exists(path))
            {
                return false;
            }
            else
            {
                value = File.ReadAllText(path);
                return true;
            }
        }

        public ICollection<string> Values
        {
            get
            {
                return (from string file in Directory.GetFiles(_path)
                        select File.ReadAllText(file)).ToList();
            }
        }

        public string this[TKey key]
        {
            get
            {
                var path = Path.Combine(_path, key.ToString());
                return File.ReadAllText(path);
            }
            set
            {
                var path = Path.Combine(_path, key.ToString());
                File.WriteAllText(path, value);
            }
        }

        public void Add(KeyValuePair<TKey, string> item)
        {
            var path = Path.Combine(_path, item.Key.ToString());
            File.WriteAllText(path, item.Value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, string> item)
        {
            var path = Path.Combine(_path, item.Key.ToString());
            return File.Exists(path);
        }

        public void CopyTo(KeyValuePair<TKey, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get {
                return Directory.GetFiles(_path).Count(); 
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, string> item)
        {
            var path = Path.Combine(_path, item.Key.ToString());
            File.Delete(path);
            return true;
        }

        public IEnumerator<KeyValuePair<TKey, string>> GetEnumerator()
        {
            return 
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Directory.GetFiles(_path).GetEnumerator();
        }
    }
}