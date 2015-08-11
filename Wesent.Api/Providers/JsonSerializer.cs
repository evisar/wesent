using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wesent.Api.Providers
{
    public class JsonSerializer<T>: ISerializer<T>
    {
        public string Serialize(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}