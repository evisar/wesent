using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Wesent.Api.Configuration
{
    [DefaultSectionName("wesent")]
    public class WesentConfiguration: ConfigurationSectionBase<WesentConfiguration>
    {
        [ConfigurationProperty("path")]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                this["path"] = value;
            }
        }

        [ConfigurationProperty("model")]
        public string Model
        {
            get
            {
                return (string)this["model"];
            }
            set
            {
                this["model"] = value;
            }
        }

        [ConfigurationProperty("persistence-provider")]
        public string PersistenceProvider
        {
            get
            {
                return (string)this["persistence-provider"];
            }
            set
            {
                this["persistence-provider"] = value;
            }
        }
    }
}