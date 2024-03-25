using System;
using System.Collections.Generic;
using VDS.RDF;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        Dictionary<string, DataSchema>? ReadUriVariables(INode propertyNode, bool propertyAffordance)
        {
            Dictionary<string, DataSchema>? uriVariables = new Dictionary<string, DataSchema>();
            IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
                propertyNode,
                graph.CreateUriNode(new Uri(TD.hasUriTemplateSchema))
            );
            foreach (Triple t in ts)
            {
                INode schemaNode = t.Object;
                string name =
                    Utils.GetObjectName(graph, schemaNode, TD.name)
                    ?? throw new Exception("mandatory uriVariable DataSchema name not found");
                DataSchema? schema = ReadDataSchema(schemaNode, propertyAffordance);
                if (schema == null)
                    continue;

                uriVariables.Add(name, schema);
            }
            return uriVariables;
        }
    }
}