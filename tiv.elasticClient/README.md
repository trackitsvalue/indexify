#Known Issues
Many Nullbale values in the Model definitions are currently commented out waiting for resolution to Issues

https://github.com/restsharp/RestSharp/issues/1018

If the expected implementation is performed then ignoring nulls will be performed by adding in the defined PocoJsonSerializationStrategy.cs file like so:

	SimpleJson.CurrentJsonSerializerStrategy = new PocoJsonSerializationStrategy();

Two examples os such usage in older versions of RestSharp are:
https://stackoverflow.com/questions/32867944/restsharp-serialize-json-in-camelcase
https://stackoverflow.com/questions/20006813/restsharp-how-to-skip-serializing-null-values-to-json
