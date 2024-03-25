using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;
using Serilog;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        List<string> ReadSecurity()
        {
            Log.Information("reading security...");
            List<string> securities = new List<string>();
            IEnumerable<Triple> securityTriples = graph.GetTriplesWithSubjectPredicate(
                baseNode,
                this.graph.CreateUriNode(new Uri(TD.hasSecurityConfiguration))
            );

            foreach (Triple t in securityTriples)
            {
                securities.Add(Utils.ParseLiteralValue(t.Object.ToString()));
            }
            if (securities.Count() == 0)
            {
                throw new Exception("mandatory security property not found");
            }
            return securities;
        }
    }
}