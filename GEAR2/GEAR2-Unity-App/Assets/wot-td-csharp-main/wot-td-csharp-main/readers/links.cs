using System;
using System.Collections.Generic;
using VDS.RDF;
using Serilog;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        List<Link>? ReadLinks()
        {
            Log.Information("reading links...");

            List<Link> links = new List<Link>();

            IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
                baseNode,
                this.graph.CreateUriNode(new Uri(TD.hasLink))
            );

            foreach (Triple t in ts)
            {
                INode linkNode = t.Object;
                string href =
                    Utils.GetObjectNode(graph, linkNode, HCTL.hasTarget)?.ToString()
                    ?? throw new Exception("mandatory link target not found");
                string? type = Utils.GetObjectName(graph, linkNode, HCTL.forContentType);
                string? rel = Utils.GetObjectName(graph, linkNode, HCTL.hasRelationType);
                string? anchor = Utils.GetObjectName(graph, linkNode, HCTL.hasAnchor);
                string? sizes = Utils.GetObjectName(graph, linkNode, HCTL.hasSizes);
                string? hreflang = Utils.GetObjectName(graph, linkNode, HCTL.hasHreflang);

                Link link = new Link(href, type, rel, anchor, sizes, hreflang);
                links.Add(link);
            }

            if (links.Count == 0)
                return null;
            else
                return links;
        }
    }
}