using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Wesent.Api.Configuration
{
    public abstract class ConfigurationSectionBase<T> : ConfigurationSection
        where T : ConfigurationSectionBase<T>, new()
    {

        static object _sync = new object();
        static string _defaultSectionName;
        static T _defaultInstance;

        static ConfigurationSectionBase()
        {
            lock (_sync)
            {
                if (_defaultSectionName == null)
                {
                    var section = typeof(T).GetCustomAttributes(typeof(DefaultSectionNameAttribute), false).FirstOrDefault() as DefaultSectionNameAttribute;
                    _defaultSectionName = section.Name;
                }
            }
        }

        public static T DefaultInstance
        {
            get
            {
                lock (_sync)
                {
                    if (_defaultInstance == null)
                    {
                        _defaultInstance = (T)ConfigurationManager.GetSection(_defaultSectionName);
                    }
                    return _defaultInstance ?? new T();
                }
            }

        }

        public string GetXml()
        {
            return this.SerializeSection(null, _defaultSectionName, ConfigurationSaveMode.Full);
        }
    }
}