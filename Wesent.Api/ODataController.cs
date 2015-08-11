using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using Wesent.Api.Configuration;
using Wesent.Api.Providers;

namespace Wesent.Api.Controllers
{
    public class ODataController<TKey, TValue> : ODataController
        where TKey: IComparable<TKey>
    {

        readonly IRepository<TKey, TValue> _repository;
        public ODataController()
        {

            _repository = new DefaultRepository<TKey, TValue>();
        }
        public IQueryable<TValue> Get(ODataQueryOptions<TValue> options)
        {
            return options.ApplyTo(_repository.Query()).OfType<TValue>();
        }
        public TValue Get([FromODataUri]TKey key)
        {
            return _repository.Get(key);
        }

        public IHttpActionResult Post(TValue obj)
        {            
            _repository.Update(obj);
            return Created(obj);
        }

        public IHttpActionResult Delete([FromODataUri]TKey key)
        {            
            _repository.Delete(key);
            return StatusCode(HttpStatusCode.NoContent);

        }
    }
}
