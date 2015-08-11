using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wesent.Api.Configuration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DefaultSectionNameAttribute : Attribute
    {
        public string Name { get; set; }

        public DefaultSectionNameAttribute(string name)
        {
            this.Name = name;
        }

        public DefaultSectionNameAttribute()
        {
        }
    }
}