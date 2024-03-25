using System;
using System.Collections.Generic;
using VDS.RDF;
using Serilog;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        List<ActionAffordance> ReadActions()
        {
            Log.Information("reading actions...");
            Log.Information("------------------");
            List<ActionAffordance> actions = new List<ActionAffordance>();

            IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
                baseNode,
                this.graph.CreateUriNode(new Uri(TD.hasActionAffordance))
            );

            // foreach actionNode
            foreach (Triple t in ts)
            {
                INode actionNode = t.Object;
                Log.Information("reading action with node: " + actionNode.ToString());
                try
                {
                    // reading mandatory properties
                    String name = Utils.ReadAffordanceName(graph, actionNode);
                    List<Form> forms =
                        ReadForms(actionNode, AffordanceType.action)
                        ?? throw new Exception("mandatory form not found");
                    // reading properties from InteractionAffordance
                    string? title = Utils.GetObjectNode(graph, actionNode, TD.title)?.ToString();
                    string? description = Utils
                        .GetObjectNode(graph, actionNode, TD.description)
                        ?.ToString();
                    Dictionary<string, DataSchema>? uriVariables = ReadUriVariables(actionNode, true);
                    // reading properties from ActionAffordance
                    DataSchema? input = ReadInputSchema(actionNode);
                    DataSchema? output = ReadOutputSchema(actionNode);
                    bool? safe = Utils.GetObjectBoolean(graph, actionNode, TD.isSafe);
                    bool? idempotent = Utils.GetObjectBoolean(graph, actionNode, TD.isIdempotent);
                    bool? synchronous = Utils.GetObjectBoolean(graph, actionNode, TD.isSynchronous);

                    // building PropertyAffordance
                    ActionAffordance.Builder builder = new ActionAffordance.Builder(name, forms);
                    if (title != null)
                        builder.AddTitle(title);
                    if (description != null)
                        builder.AddDescription(description);
                    if (uriVariables != null)
                        builder.AddAllUriVarirables(uriVariables);
                    if (input != null)
                        builder.AddInput(input);
                    if (output != null)
                        builder.AddOutput(output);
                    if (safe != null)
                        builder.SetSafe(safe.Value);
                    if (idempotent != null)
                        builder.SetIdempotent(idempotent.Value);
                    if (synchronous != null)
                        builder.SetSynchronous(synchronous.Value);
                    actions.Add(builder.build());
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid property definition.", e);
                }
            }

            Log.Information(actions.Count + " actions found");
            return actions;
        }

        DataSchema? ReadInputSchema(INode actionNode)
        {
            INode? inputNode = Utils.GetObjectNode(graph, actionNode, TD.hasInputSchema);

            if (inputNode == null)
                return null;
            return ReadDataSchema(inputNode, false);
        }

        DataSchema? ReadOutputSchema(INode actionNode)
        {
            INode? outputNode = Utils.GetObjectNode(graph, actionNode, TD.hasOutputSchema);

            if (outputNode == null)
                return null;
            return ReadDataSchema(outputNode, false);
        }
    }
}