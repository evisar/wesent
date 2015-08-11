using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wesent.Api.Providers
{
    public interface ISerializer<T>
    {
        string Serialize(T value);
        T Deserialize(string value);
    }
}