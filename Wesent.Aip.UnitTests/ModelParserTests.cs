using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Collections.Generic;
using Wesent.Api.Configuration;
using Wesent.Api;
using Wesent.Api.Common;

namespace Wesent.Aip.UnitTests
{
    [TestClass]
    public class ModelParserTests
    {



        [TestMethod]
        public void ParseEdmx_ProducesCorrectResults()
        {
            //arrange
            var config = WesentConfiguration.DefaultInstance;            
            var edmx = new XmlDocument();
            edmx.Load(config.Model);

            //act
            string @namespace;
            var model = new EdmxParser(edmx).Parse(out @namespace);

        }
    }
}
