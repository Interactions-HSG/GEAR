using System;
using System.Linq;
using VDS.RDF;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        string? ReadThingId()
        {
            if (baseNode is BlankNode)
                return null;
            else
                return baseNode.ToString();
        }

        Version? ReadVersion()
        {
            INode? versionNode = graph
                .GetTriplesWithSubjectPredicate(baseNode, graph.CreateUriNode(new Uri(TD.versionInfo)))
                .FirstOrDefault()
                ?.Object;

            if (versionNode != null)
            {
                string instance =
                    Utils.GetObjectNode(graph, versionNode, TD.instance)?.ToString()
                    ?? throw new Exception("mandatory version instance not found");
                string? model = Utils.GetObjectNode(graph, versionNode, TD.model)?.ToString();
                return new Version(instance, model);
            }
            else
                return null;
        }
    }
}