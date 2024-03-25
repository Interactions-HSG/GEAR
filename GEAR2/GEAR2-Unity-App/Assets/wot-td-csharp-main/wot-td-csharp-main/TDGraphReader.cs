using VDS.RDF;
using VDS.RDF.Parsing;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Serilog;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        readonly INode baseNode;
        Graph graph;
        readonly TDFormat fileFormat;

        public static ThingDescription ReadFromUri(Uri uri, TDFormat fileFormat)
        {
            TDGraphReader reader = new TDGraphReader(uri, fileFormat);
            ThingDescription td = ParseThingDescription(reader);
            return td;
        }

        public static ThingDescription ReadFromFile(string filePath, TDFormat fileFormat)
        {
            TDGraphReader reader = new TDGraphReader(filePath, fileFormat);
            ThingDescription td = ParseThingDescription(reader);
            return td;
        }

        public static ThingDescription ParseThingDescription(TDGraphReader reader)
        {
            ThingDescription.Builder tdBuilder = new ThingDescription.Builder(
                reader.ReadThingTitle(),
                reader.ReadSecurity(),
                reader.ReadSecurityDefinitions()
            )
                .AddThingProperties(reader.ReadProperties())
                .AddThingActions(reader.ReadActions())
                .AddThingEvents(reader.ReadEvents());

            string? thingId = reader.ReadThingId();
            string? description = Utils
                .GetObjectNode(reader.graph, reader.baseNode, TD.description)
                ?.ToString();
            Version? version = reader.ReadVersion();
            string? created = Utils.GetObjectName(
                reader.graph,
                reader.baseNode,
                "http://purl.org/dc/terms/created"
            );
            string? modified = Utils.GetObjectName(
                reader.graph,
                reader.baseNode,
                "http://purl.org/dc/terms/modified"
            );
            string? support = Utils.GetObjectName(reader.graph, reader.baseNode, TD.supportContact);
            string? base_ = Utils.GetObjectName(reader.graph, reader.baseNode, TD.baseURI);
            List<Link>? links = reader.ReadLinks();
            List<Form>? forms = reader.ReadForms(reader.baseNode);
            List<string>? profile = Utils.GetObjectNames(
                reader.graph,
                reader.baseNode,
                TD.followsProfile
            );
            // jsonld parsed graph is weird for schemaDefinitions
            // Dictionary<string, DataSchema>? schemaDefinitions = reader.ReadSchemaDefinitions();
            // jsonld parsed graph does not work or false configuration for uriVariables
            // Dictionary<string, DataSchema>? uriVariables = reader.ReadUriVariables(
            //     reader.baseNode,
            //     false
            // );

            if (thingId != null)
                tdBuilder.SetThingId(thingId);
            if (description != null)
                tdBuilder.SetDescription(description);
            if (version != null)
                tdBuilder.SetVersion(version);
            if (created != null)
                tdBuilder.SetCreated(created);
            if (modified != null)
                tdBuilder.SetModified(modified);
            if (support != null)
                tdBuilder.SetSupport(support);
            if (base_ != null)
                tdBuilder.SetBase(base_);
            if (links != null)
                tdBuilder.AddLinks(links);
            if (forms != null)
                tdBuilder.AddForms(forms);
            if (profile != null)
                tdBuilder.AddProfile(profile);
            // if (schemaDefinitions != null)
            //     tdBuilder.AddSchemaDefinitions(schemaDefinitions);
            // if (uriVariables != null)
            //     tdBuilder.AddUriVariables(uriVariables);

            ThingDescription td = tdBuilder.Build();

            // Log.Information("ThingDescription:");
            // Log.Information("thingId: " + td.id);
            // Log.Information("title: " + td.title);
            // Log.Information("description: " + td.description);
            // Log.Information("version: " + td.version);
            // Log.Information("created: " + td.created);
            // Log.Information("modified: " + td.modified);
            // Log.Information("support: " + td.support);
            // Log.Information("base: " + td.base_);

            // // print complex properties
            // foreach (string security in td.security)
            // {
            //     Log.Information("security: " + security);
            // }

            // Log.Information("Links:");
            // foreach (Link link in td.links)
            // {
            //     Utils.PrintLink(link);
            // }

            // Log.Information("Forms:");
            // foreach (Form form in td.forms)
            // {
            //     Utils.PrintForm(form);
            // }

            // Log.Information("profile:");
            // foreach (string profile_ in td.Profile)
            // {
            //     Log.Information(profile_);
            // }

            Log.Information("Thing parsed successfully!");
            Console.ResetColor();
            Log.Information("------------------");
            return td;
        }

        TDGraphReader(Uri uri, TDFormat fileFormat)
        {
            this.fileFormat = fileFormat;

            LoadModel(uri, fileFormat);

            if (graph == null)
            {
                throw new Exception("graph is not loaded");
            }
            // print all the triples
            // foreach (Triple t in graph.Triples)
            // {
            //     Log.Information(t.ToString());
            // }
            // Log.Information("------------------");

            // get the baseNode
            Triple? baseNodeTriple = graph
                .GetTriplesWithPredicate(graph.CreateUriNode(new Uri(TD.hasSecurityConfiguration)))
                .FirstOrDefault();

            if (baseNodeTriple == null)
                throw new Exception("mandatory security not found");
            else
                baseNode = baseNodeTriple.Subject;

            Log.Information("baseNode: " + baseNode);
        }

        TDGraphReader(string filePath, TDFormat fileFormat)
        {
            this.fileFormat = fileFormat;

            LoadModel(filePath, fileFormat);

            if (graph == null)
            {
                throw new Exception("graph is not loaded");
            }
            // print all the triples
            // foreach (Triple t in graph.Triples)
            // {
            //     Log.Information(t.ToString());
            // }
            // Log.Information("------------------");

            // get the baseNode
            Triple? baseNodeTriple = graph
                .GetTriplesWithPredicate(graph.CreateUriNode(new Uri(TD.hasSecurityConfiguration)))
                .FirstOrDefault();

            if (baseNodeTriple == null)
                throw new Exception("mandatory security not found");
            else
                baseNode = baseNodeTriple.Subject;

            Log.Information("baseNode: " + baseNode);
        }

        void LoadModel(string filePath, TDFormat fileFormat)
        {
            Log.Information("loading model...");
            Graph graph = new Graph();

            if (fileFormat == TDFormat.jsonld)
            {
                JsonLdParser parser = new JsonLdParser();
                TripleStore store = new TripleStore();
                parser.Load(store, filePath);

                // create a graph
                foreach (Triple t in store.Triples)
                {
                    graph.Assert(t);
                }
            }
            else if (fileFormat == TDFormat.ttl)
            {
                TurtleParser parser = new TurtleParser();
                parser.Load(graph, filePath);
            }
            this.graph = graph;
        }

        void LoadModel(Uri uri, TDFormat fileFormat)
        {
            Log.Information("loading model...");

            Graph graph = new Graph();
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = client.GetAsync(uri).Result;
                response.EnsureSuccessStatusCode();
                string content = response.Content.ReadAsStringAsync().Result;
                if (fileFormat == TDFormat.jsonld)
                {

                    // workaround because dotnetrdf only supports file input
                    // save content to file
                    string fileName = "ThingsDescriptionTemp.jsonld";
                    File.WriteAllText(fileName, content);
                    // load from file
                    JsonLdParser parser = new JsonLdParser();
                    TripleStore store = new TripleStore();
                    parser.Load(store, fileName);

                    // create a graph
                    foreach (Triple t in store.Triples)
                    {
                        graph.Assert(t);
                    }
                    File.Delete(fileName);
                }
                else if (fileFormat == TDFormat.ttl)
                {
                    TurtleParser parser = new TurtleParser();
                    parser.Load(graph, content);
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception("The HTTP request to the TD URI failed: " + e.Message);
            }

            this.graph = graph;
        }
    }



}