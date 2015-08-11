# wesent
Dynamic OData service using ESENT repository (this is work in progress)
The ultimate idea is to allow you to literally draw and prototype your OData API, in the least amount of time.

This was inspired by DeployD.
By comparison, this uses ESENT as repository, and EF modeler for designing data servies.
It has full compatibility for LinqPad, Excel, Office, etc, since it's using OData.

Current version od OData controller does not support metadata only entities, 
thus I create and cache types on the fly with type builder.

ESENT is not hardwired, there is IRepository<T> interface, which 
could be plugged-in (not yet written the config attribute) and persist data in File, Mongo, Azure, etc.

It has currently support for authentication with Username Credentials only.
