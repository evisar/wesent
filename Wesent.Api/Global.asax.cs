using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.OData.Builder;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml;
using Wesent.Api.Common;
using Wesent.Api.Configuration;

namespace Wesent.Api
{
    public class Global : HttpApplication
    {

        public void Application_Start()
        {
            var edmx = new XmlDocument();
            edmx.Load(WesentConfiguration.DefaultInstance.Model);
            var modelParser = new EdmxParser(edmx);

            var types = new RuntimeTypeGenerator(modelParser).GenerateTypes(); 

            GlobalConfiguration.Configure(config =>
            {

                //config.MessageHandlers.Add(new BasicAuthenticationHandler());
                //config.Filters.Add(new BasicAuthorizationFilter());

                ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

                config.Services.Replace(typeof(IHttpControllerSelector), new DynamicControllerSelector(config, types));

                foreach (var kv in types)
                {
                    var mi = typeof(ODataConventionModelBuilder).GetMethod("EntitySet").MakeGenericMethod(kv.Value); ;
                    mi.Invoke(builder, new[] { kv.Key });
                }
                config.Routes.MapODataRoute("odata", "", builder.GetEdmModel());

                File.AppendAllLines("C:\\LogFiles\\wesent\\log.log", new[] { DateTime.Now.ToString() });


            });
        }
    }
}