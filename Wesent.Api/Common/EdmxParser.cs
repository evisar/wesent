using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Wesent.Api.Common
{
    public class EdmxParser
    {
        static readonly IDictionary<string, Type> _typeMap = new Dictionary<string, Type>
            {
              {"Int32", typeof(int)},
              {"Guid", typeof(Guid)},
              {"String", typeof(string)}
            };


        readonly XmlDocument _edmx;
        public EdmxParser(XmlDocument edmx)
        {
            this._edmx = edmx;
        }

        public IDictionary<string,IDictionary<string,Tuple<Type,bool>>> Parse(out string @namespace)
        {
            var model = new Dictionary<string, IDictionary<string, Tuple<Type,bool>>>();

            var nsmgr = new XmlNamespaceManager(_edmx.NameTable);
            nsmgr.AddNamespace("edmx", "http://schemas.microsoft.com/ado/2009/11/edmx");
            nsmgr.AddNamespace("edm", "http://schemas.microsoft.com/ado/2009/11/edm");
            var schema = _edmx.SelectSingleNode("//edmx:ConceptualModels/edm:Schema", nsmgr);
            @namespace = schema.Attributes["Namespace"].Value;

            var entitySetsNodes = _edmx.SelectNodes("//edmx:ConceptualModels/edm:Schema/edm:EntityContainer/edm:EntitySet", nsmgr);
            foreach (XmlNode entitySetNode in entitySetsNodes)
            {
                var name = entitySetNode.Attributes["Name"].Value;
                var type = entitySetNode.Attributes["EntityType"].Value;
                model[name] = new Dictionary<string, Tuple<Type,bool>>();

                var entityName = type.Replace(@namespace + ".", "");

                var propertyNodes = _edmx.SelectNodes(string.Format("//edmx:ConceptualModels/edm:Schema/edm:EntityType[@Name='{0}']/edm:Property", entityName), nsmgr);

                foreach (XmlNode propertyNode in propertyNodes)
                {
                    var propertyName = propertyNode.Attributes["Name"].Value;
                    var propertyType = propertyNode.Attributes["Type"].Value;
                    var isKey = _edmx.SelectSingleNode(string.Format("//edmx:ConceptualModels/edm:Schema/edm:EntityType[@Name='{0}']/edm:Key/edm:PropertyRef[@Name='{1}']", entityName, propertyName), nsmgr)!=null; 
                    model[name][propertyName] = new Tuple<Type,bool>(_typeMap[propertyType],isKey);
                }
            }

            return model;
        }
    }
}