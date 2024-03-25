using System;
using System.Collections.Generic;
using VDS.RDF;
using Serilog;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        List<PropertyAffordance> ReadProperties()
        {
            Log.Information("reading properties...");
            Log.Information("------------------");
            List<PropertyAffordance> properties = new List<PropertyAffordance>();

            IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
                baseNode,
                this.graph.CreateUriNode(new Uri(TD.hasPropertyAffordance))
            );

            // foreach propertyNode
            foreach (Triple t in ts)
            {
                INode propertyNode = t.Object;
                Log.Information("reading property with node: " + propertyNode.ToString());
                try
                {
                    // reading mandatory properties
                    String name = Utils.ReadAffordanceName(graph, propertyNode);
                    Log.Information("reading data schema...");
                    DataSchema? schema = ReadDataSchema(propertyNode, true);
                    List<Form> forms =
                        ReadForms(
                            propertyNode,
                            AffordanceType.property,
                            schema?.readOnly,
                            schema?.writeOnly
                        ) ?? throw new Exception("mandatory form not found");
                    // reading properties from InteractionAffordance
                    string? title = Utils.GetObjectNode(graph, propertyNode, TD.title)?.ToString();
                    string? description = Utils
                        .GetObjectNode(graph, propertyNode, TD.description)
                        ?.ToString();
                    Dictionary<string, DataSchema>? uriVariables = ReadUriVariables(propertyNode, true);
                    // reading properties from PropertyAffordance
                    bool? observable = Utils.GetObjectBoolean(graph, propertyNode, TD.isObservable);
                    // reading properties from DataSchema

                    // building PropertyAffordance
                    PropertyAffordance.Builder builder = new PropertyAffordance.Builder(name, forms);
                    if (title != null)
                        builder.AddTitle(title);
                    if (description != null)
                        builder.AddDescription(description);
                    if (uriVariables != null)
                        builder.AddAllUriVarirables(uriVariables);
                    if (observable != null)
                        builder.SetObservable(observable);
                    if (schema != null)
                        builder.AddDataSchema(schema);
                    PropertyAffordance property = builder.build();

                    properties.Add(builder.build());
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid property definition.", e);
                }
            }

            Log.Information(properties.Count + " properties found");
            return properties;
        }
    }
}