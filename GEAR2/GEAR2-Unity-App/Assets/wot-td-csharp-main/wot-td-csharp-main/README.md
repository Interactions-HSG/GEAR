# wot-td-csharp

This repo is for parsing a [Thing Description](https://www.w3.org/TR/wot-thing-description11/), version 1.1 in the format of JSONLD and TURTLE.

It generates a class with executable http requests to properties, actions and events to enable a client to automatically interact with the Thing in an intuitive manner. The class is structured as described in the Thing Description [documentation](https://www.w3.org/TR/wot-thing-description11/). Almost all fields should have these exact names. Also, the default values are automatically set.

## Getting Started

Add the Library as a dependency to your C# project using e.g. Visual Studio

Four basic parsing methods are available:

-   parsing a Thing Description from a .jsonld file

```
ThingDescription td = TDGraphReader.ReadFromFile(filePath, TDFormat.jsonld);
```

-   parsing from an URL that returns a Thing Description in the form of JSONLD

```csharp
ThingDescription td = TDGraphReader.ReadFromUri(new Uri(uri), TDFormat.jsonld);
```

-   parsing a Thing Description from a .ttl file

```csharp
ThingDescription td = TDGraphReader.ReadFromFile(filePath, TDFormat.ttl);
```

-   parsing from an URL that returns a Thing Description in the form of RDF

```csharp
ThingDescription td = TDGraphReader.ReadFromUri(new Uri(uri), TDFormat.ttl);
```

After the Thing is parsed, use the generated ThingDescription instance to view it's fields and make requests to the Thing.

## executing HTTP Requests

```csharp
List<string>? propertyNames = td.GetPropertyNames();
if (propertyNames != null)
{
    // get a property by name
    PropertyAffordance? property1 = td.GetPropertyByName(propertyNames[0]);
    if (property1 != null)
    {
        // get the property value from the thing
        string response = await property1.GetPropertValue();
        // set the property value
        string response2 = await property1.SetPropertyValue("on");
    }
}
```

## Logging

If you want to see the logs from the library, initialize a Serilog logger:

```
using Serilog;
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
```
