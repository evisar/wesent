using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Web;
using System.Web.Services.Description;
using System.Xml;
using Wesent.Api.Configuration;
using Wesent.Api.Controllers;

namespace Wesent.Api.Common
{
    public class RuntimeTypeGenerator
    {
        static readonly object _sync = new object();
        static readonly IDictionary<string, Type> _types = new Dictionary<string, Type>();

        readonly EdmxParser _modelParser;
        public RuntimeTypeGenerator(EdmxParser modelParser)
        {
            this._modelParser = modelParser;
        }

        public IDictionary<string,Type> GenerateTypes()
        {

            lock (_sync)
            {                
                string @namespace;
                var model = _modelParser.Parse(out @namespace);


                // create a dynamic assembly and module 
                AssemblyName assemblyName = new AssemblyName();
                assemblyName.Name = @namespace;
                System.Reflection.Emit.AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                ModuleBuilder module = assemblyBuilder.DefineDynamicModule(@namespace);

                foreach (var typeConfig in model)
                {

                    // create a new type builder
                    TypeBuilder typeBuilder = module.DefineType(@namespace + "." + typeConfig.Key, TypeAttributes.Public |
                        TypeAttributes.Sealed | TypeAttributes.Class |
                        TypeAttributes.Serializable);


                    foreach (var propConfig in model[typeConfig.Key])
                    {

                        FieldBuilder field = typeBuilder.DefineField("_" + propConfig.Key, propConfig.Value.Item1, FieldAttributes.Private);
                        // Generate a public property
                        PropertyBuilder property =
                            typeBuilder.DefineProperty(propConfig.Key,
                                             PropertyAttributes.None,
                                             propConfig.Value.Item1,
                                             null);

                        // The property set and property get methods require a special set of attributes:

                        MethodAttributes GetSetAttr =
                            MethodAttributes.Public |
                            MethodAttributes.HideBySig;

                        // Define the "get" accessor method for current private field.
                        MethodBuilder currGetPropMthdBldr =
                            typeBuilder.DefineMethod("get_value",
                                                       GetSetAttr,
                                                       propConfig.Value.Item1,
                                                       Type.EmptyTypes);

                        // Intermediate Language stuff...
                        ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
                        currGetIL.Emit(OpCodes.Ldarg_0);
                        currGetIL.Emit(OpCodes.Ldfld, field);
                        currGetIL.Emit(OpCodes.Ret);

                        // Define the "set" accessor method for current private field.
                        MethodBuilder currSetPropMthdBldr =
                            typeBuilder.DefineMethod("set_value",
                                                       GetSetAttr,
                                                       null,
                                                       new Type[] { propConfig.Value.Item1 });

                        // Again some Intermediate Language stuff...
                        ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
                        currSetIL.Emit(OpCodes.Ldarg_0);
                        currSetIL.Emit(OpCodes.Ldarg_1);
                        currSetIL.Emit(OpCodes.Stfld, field);
                        currSetIL.Emit(OpCodes.Ret);

                        // Last, we must map the two methods created above to our PropertyBuilder to 
                        // their corresponding behaviors, "get" and "set" respectively. 
                        property.SetGetMethod(currGetPropMthdBldr);
                        property.SetSetMethod(currSetPropMthdBldr);

                        if (propConfig.Value.Item2)
                        {
                            var attrCtorInfo = typeof(KeyAttribute).GetConstructor(Type.EmptyTypes);
                            var attrBuilder = new CustomAttributeBuilder(attrCtorInfo, new object[] { });
                            property.SetCustomAttribute(attrBuilder);
                        }
                    }

                    var type = typeBuilder.CreateType();
                    _types[typeConfig.Key] = type;
                }
            }

            return _types;
        }
    }
}