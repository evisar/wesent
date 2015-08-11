using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wesent.Api.Providers
{
    public interface IRepository<TKey, TValue>
    {
        IQueryable<TValue> Query();

        TValue Get(TKey key);
        void Update(TValue value);
        void Delete(TKey key);
    }
}
