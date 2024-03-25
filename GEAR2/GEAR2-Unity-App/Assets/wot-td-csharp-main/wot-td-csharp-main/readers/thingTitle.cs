using System;
using System.Linq;
using Serilog;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        string ReadThingTitle()
        {
            Log.Information("reading title...");
            string title =
                graph
                    .GetTriplesWithSubjectPredicate(
                        baseNode,
                        this.graph.CreateUriNode(new Uri(TD.title))
                    )
                    ?.FirstOrDefault()
                    ?.Object?.ToString() ?? throw new Exception("mandatory title not found");
            Log.Information("title: " + title);
            return title;
        }
    }
}