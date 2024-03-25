using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;
// using Serilog;

public static partial class Utils
{
    public static string ReadAffordanceName(Graph graph, INode affordanceNode)
    {
        string ts =
            graph
                .GetTriplesWithSubjectPredicate(
                    affordanceNode,
                    graph.CreateUriNode(new Uri(TD.name))
                )
                .FirstOrDefault()
                ?.Object.ToString() ?? throw new Exception("mandatory affordance name not found");

        return Utils.ParseLiteralValue(ts);
    }

    public static string? GetObjectName(Graph graph, INode subject, string predicate)
    {
        string? objectString = GetObjectString(graph, subject, predicate);
        if (objectString == null)
            return null;
        return ParseLiteralValue(objectString);
    }

    public static List<string>? GetObjectNames(Graph graph, INode subject, string predicate)
    {
        IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
            subject,
            graph.CreateUriNode(new Uri(predicate))
        );
        if (ts.Count() == 0)
            return null;

        List<string> names = new List<string>();
        foreach (Triple t in ts)
        {
            names.Add(ParseLiteralValue(t.Object.ToString()));
        }
        return names;
    }

    public static int? GetObjectInt(Graph graph, INode subject, string predicate)
    {
        string? objectString = GetObjectString(graph, subject, predicate);
        if (objectString == null)
            return null;
        return int.Parse(ParseLiteralValue(objectString));
    }

    public static double? GetObjectFloat(Graph graph, INode subject, string predicate)
    {
        string? objectString = GetObjectString(graph, subject, predicate);
        if (objectString == null)
            return null;
        return double.Parse(ParseLiteralValue(objectString));
    }

    public static INode? GetObjectNode(Graph graph, INode subject, string predicate)
    {
        return graph
            .GetTriplesWithSubjectPredicate(subject, graph.CreateUriNode(new Uri(predicate)))
            .FirstOrDefault()
            ?.Object;
    }

    public static bool? GetObjectBoolean(Graph graph, INode subject, string predicate)
    {
        string? objectString = GetObjectString(graph, subject, predicate);
        if (objectString == null)
            return null;
        return bool.Parse(ParseLiteralValue(objectString));
    }

    public static string? GetObjectString(Graph graph, INode subject, string predicate)
    {
        string? objectString = graph
            .GetTriplesWithSubjectPredicate(subject, graph.CreateUriNode(new Uri(predicate)))
            .FirstOrDefault()
            ?.Object?.ToString();
        if (objectString == null)
            return null;
        return objectString;
    }

    public static string ParseLiteralValue(string literal)
    {
        literal = literal.Split('^')[0];
        return literal;
    }

    public static string ParseNodeName(string node)
    {
        int index = node.LastIndexOf('#');
        return node[(index + 1)..];
    }

    public static DataSchemaType ParseNodeNameToDataType(string node)
    {
        int index = node.LastIndexOf('#');
        string type = node[(index + 1)..];
        return (DataSchemaType)Enum.Parse(typeof(DataSchemaType), type);
    }

    // public static void PrintDataSchema(DataSchema? dataSchema)
    // {
    //     if (dataSchema == null)
    //     {
    //         Log.Information("dataSchema is null");
    //         return;
    //     }

    //     if (dataSchema is ObjectSchema ObjectSchema)
    //     {
    //         if (ObjectSchema.properties != null)
    //         {
    //             foreach (KeyValuePair<string, DataSchema> kvp in ObjectSchema.properties)
    //             {
    //                 Log.Information("name: " + kvp.Key + " | datatype: " + kvp.Value.dataType);
    //                 if (kvp.Value is ObjectSchema objectSchema)
    //                 {
    //                     if (objectSchema.properties != null)
    //                     {
    //                         Log.Information("properties:");
    //                         foreach (
    //                             KeyValuePair<string, DataSchema> kvp2 in objectSchema.properties
    //                         )
    //                         {
    //                             Console.Write("name: " + kvp2.Key + " ");
    //                             PrintDataSchema(kvp2.Value);
    //                         }
    //                     }
    //                 }
    //             }

    //             Log.Information("------------------");
    //         }
    //         else
    //         {
    //             Log.Information("datatype: " + dataSchema.dataType);
    //             Log.Information("properties is null");
    //         }
    //     }
    //     else
    //     {
    //         Log.Information("datatype: " + dataSchema.dataType);
    //     }
    // }

    // public static void PrintSecuritySchemes(Dictionary<string, SecurityScheme>? securitySchemes)
    // {
    //     if (securitySchemes != null)
    //     {
    //         foreach (KeyValuePair<string, SecurityScheme> kvp in securitySchemes)
    //         {
    //             Log.Information("title: " + kvp.Key);
    //             Log.Information("scheme: " + kvp.Value.scheme);
    //             if (kvp.Value.description != null)
    //                 Log.Information("description: " + kvp.Value.description);
    //             if (kvp.Value.proxy != null)
    //                 Log.Information("proxy: " + kvp.Value.proxy);

    //             if (kvp.Value is ComboSecurityScheme css)
    //             {
    //                 foreach (string s in css.oneOf)
    //                 {
    //                     Log.Information("oneOf: " + s);
    //                 }
    //                 foreach (string s in css.allOf)
    //                 {
    //                     Log.Information("allOf: " + s);
    //                 }
    //             }
    //             if (kvp.Value is BasicSecurityScheme)
    //             {
    //                 BasicSecurityScheme bss = (BasicSecurityScheme)kvp.Value;
    //                 Log.Information("name: " + bss.name);
    //                 Log.Information("in: " + bss.in_);
    //             }
    //             if (kvp.Value is DigestSecurityScheme dss)
    //             {
    //                 Log.Information("name: " + dss.name);
    //                 Log.Information("in: " + dss.in_);
    //                 Log.Information("qop: " + dss.qop);
    //             }
    //             if (kvp.Value is APIKeySecurityScheme akss)
    //             {
    //                 Log.Information("in: " + akss.in_);
    //                 Log.Information("name: " + akss.name);
    //             }
    //             if (kvp.Value is BearerSecurityScheme)
    //             {
    //                 BearerSecurityScheme bss = (BearerSecurityScheme)kvp.Value;
    //                 Log.Information("authorization: " + bss.authorization);
    //                 Log.Information("name: " + bss.name);
    //                 Log.Information("alg: " + bss.alg);
    //                 Log.Information("format: " + bss.format);
    //                 Log.Information("in: " + bss.in_);
    //             }
    //             if (kvp.Value is PSKSecurityScheme pss)
    //             {
    //                 Log.Information("identity: " + pss.identity);
    //             }
    //             if (kvp.Value is OAuth2SecurityScheme o2ss)
    //             {
    //                 Log.Information("authorization: " + o2ss.authorization);
    //                 Log.Information("token: " + o2ss.token);
    //                 Log.Information("refresh: " + o2ss.refresh);
    //                 Log.Information("scopes: " + o2ss.scopes);
    //                 Log.Information("flow: " + o2ss.flow);
    //             }
    //             Log.Information("------------------");
    //         }
    //     }
    // }

    // public static void PrintForm(Form form)
    // {
    //     Log.Information("href: " + form.href);
    //     if (form.contentType != null)
    //         Log.Information("contentType: " + form.contentType);
    //     if (form.contentCoding != null)
    //         Log.Information("contentCoding: " + form.contentCoding);
    //     if (form.security != null)
    //         foreach (string security in form.security)
    //         {
    //             Log.Information("security: " + security);
    //         }
    //     if (form.scopes != null)
    //         foreach (string scope in form.scopes)
    //         {
    //             Log.Information("scope: " + scope);
    //         }
    //     if (form.response != null)
    //         Log.Information("response content type: " + form.response.contentType);
    //     if (form.additionalResponses != null)
    //     {
    //         Log.Information("additional responses:");
    //         foreach (AdditionalExpectedResponse aer in form.additionalResponses)
    //         {
    //             Log.Information("additional response success: " + aer.success);
    //             Log.Information("additional response content type: " + aer.contentType);
    //         }
    //     }
    //     if (form.subprotocol != null)
    //         Log.Information("subprotocol: " + form.subprotocol);
    //     if (form.op != null)
    //         foreach (Op op in form.op)
    //         {
    //             Log.Information("op: " + op);
    //         }
    //     Log.Information("------------------");
    // }

    // public static void PrintLink(Link link)
    // {
    //     Log.Information("href: " + link.href);
    //     if (link.rel != null)
    //         Log.Information("rel: " + link.rel);
    //     if (link.type != null)
    //         Log.Information("type: " + link.type);
    //     if (link.hreflang != null)
    //         Log.Information("hreflang: " + link.hreflang);
    // }
    // public static void PrintDictionary(Dictionary<string, object> dictionary, string indent)
    // {
    //     foreach (var kvp in dictionary)
    //     {
    //         Log.Information($"{indent}{kvp.Key}: {kvp.Value}");

    //         if (kvp.Value is Dictionary<string, object> subDict)
    //         {
    //             PrintDictionary(subDict, indent + "  ");
    //         }
    //     }
    // }
}
