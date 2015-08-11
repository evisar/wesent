using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.OData;
using Wesent.Api.Controllers;

namespace Wesent.Api
{
    public class DynamicControllerSelector : DefaultHttpControllerSelector
    {
        readonly HttpConfiguration _configuration;
        readonly IDictionary<string, Type> _types;
        
 
        public DynamicControllerSelector(HttpConfiguration configuration, IDictionary<string,Type> types )
            : base(configuration)
        {
            _configuration = configuration;
            _types = types;
        }
 
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        { 
            var controllerName = base.GetControllerName(request);

            if (controllerName == "ODataMetadata")
            {
                return new HttpControllerDescriptor(_configuration, controllerName, typeof(ODataMetadataController));
            }
            else
            {                                
                var valueType = _types[controllerName];
                Type keyType = null;
                foreach (var pi in valueType.GetProperties())
                {
                    if (pi.GetCustomAttribute(typeof(KeyAttribute), false) != null)
                    {
                        keyType = pi.PropertyType;
                        break;
                    }
                }
                var controllerType = typeof(ODataController<,>).MakeGenericType(keyType, valueType);
                return new HttpControllerDescriptor(_configuration, controllerName, controllerType);
            }
        }
    }
}