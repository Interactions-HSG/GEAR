using System;
using System.Collections.Generic;
using VDS.RDF;
using Serilog;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        List<EventAffordance> ReadEvents()
        {
            Log.Information("reading events...");
            Log.Information("------------------");
            List<EventAffordance> events = new List<EventAffordance>();

            IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
                baseNode,
                this.graph.CreateUriNode(new Uri(TD.hasEventAffordance))
            );

            // foreach actionNode
            foreach (Triple t in ts)
            {
                INode actionNode = t.Object;
                Log.Information("reading event with node: " + actionNode.ToString());
                try
                {
                    // reading mandatory properties
                    String name = Utils.ReadAffordanceName(graph, actionNode);
                    List<Form> forms =
                        ReadForms(actionNode, AffordanceType.event_)
                        ?? throw new Exception("mandatory form not found");
                    // reading properties from InterEventAffordance
                    string? title = Utils.GetObjectNode(graph, actionNode, TD.title)?.ToString();
                    string? description = Utils
                        .GetObjectNode(graph, actionNode, TD.description)
                        ?.ToString();
                    Dictionary<string, DataSchema>? uriVariables = ReadUriVariables(actionNode, true);
                    // reading properties from EventAffordance
                    DataSchema? subscription = ReadSubscriptionSchema(actionNode);
                    DataSchema? data = ReadDataDataSchema(actionNode);
                    DataSchema? dataResponse = ReadDataResponseSchema(actionNode);
                    DataSchema? cancellation = ReadCancellationSchema(actionNode);

                    // building PropertyAffordance
                    EventAffordance.Builder builder = new EventAffordance.Builder(name, forms);
                    if (title != null)
                        builder.AddTitle(title);
                    if (description != null)
                        builder.AddDescription(description);
                    if (uriVariables != null)
                        builder.AddAllUriVarirables(uriVariables);
                    if (subscription != null)
                        builder.AddSubscription(subscription);
                    if (data != null)
                        builder.AddData(data);
                    if (dataResponse != null)
                        builder.AddDataResponse(dataResponse);
                    if (cancellation != null)
                        builder.AddCancellation(cancellation);
                    events.Add(builder.build());
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid property definition.", e);
                }
            }

            Log.Information(events.Count + " events found");
            return events;
        }

        DataSchema? ReadSubscriptionSchema(INode actionNode)
        {
            INode? subscriptionNode = Utils.GetObjectNode(graph, actionNode, TD.hasSubscriptionSchema);

            if (subscriptionNode == null)
                return null;
            return ReadDataSchema(subscriptionNode, false);
        }

        DataSchema? ReadDataDataSchema(INode actionNode)
        {
            INode? dataNode = Utils.GetObjectNode(graph, actionNode, TD.hasNotificationSchema);

            if (dataNode == null)
                return null;
            return ReadDataSchema(dataNode, false);
        }

        DataSchema? ReadDataResponseSchema(INode actionNode)
        {
            INode? dataResponseNode = Utils.GetObjectNode(
                graph,
                actionNode,
                TD.hasNotificationResponseSchema
            );

            if (dataResponseNode == null)
                return null;
            return ReadDataSchema(dataResponseNode, false);
        }

        DataSchema? ReadCancellationSchema(INode actionNode)
        {
            INode? cancellationNode = Utils.GetObjectNode(graph, actionNode, TD.hasCancellationSchema);

            if (cancellationNode == null)
                return null;
            return ReadDataSchema(cancellationNode, false);
        }
    }
}
