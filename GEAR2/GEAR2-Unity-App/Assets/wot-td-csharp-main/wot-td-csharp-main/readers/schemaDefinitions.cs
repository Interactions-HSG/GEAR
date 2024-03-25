using System;
using System.Collections.Generic;
using VDS.RDF;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        Dictionary<string, DataSchema>? ReadSchemaDefinitions()
        {
            Dictionary<string, DataSchema>? schemaDefinitions = new Dictionary<string, DataSchema>();
            IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
                baseNode,
                graph.CreateUriNode(new Uri(TD.schemaDefinitions))
            );
            foreach (Triple t in ts)
            {
                INode schemaNode = t.Object;
                string name =
                    Utils.GetObjectName(graph, schemaNode, TD.name)
                    ?? throw new Exception("mandatory schemaDefinition DataSchema name not found");
                DataSchema? schema = ReadDataSchema(schemaNode, false);
                if (schema == null)
                    continue;

                schemaDefinitions.Add(name, schema);
            }
            return schemaDefinitions;
        }
    }
}