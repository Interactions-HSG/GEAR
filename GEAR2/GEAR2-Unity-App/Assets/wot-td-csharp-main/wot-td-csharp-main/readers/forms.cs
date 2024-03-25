using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;
using Serilog;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        List<Form>? ReadForms(
            INode startNode,
            AffordanceType? affordanceType = null,
            bool? readOnly = null,
            bool? writeOnly = null
        )
        {
            Log.Information("reading forms...");
            List<Form> forms = new List<Form>();

            IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
                startNode,
                this.graph.CreateUriNode(new Uri(TD.hasForm))
            );

            foreach (Triple t in ts)
            {
                INode formNode = t.Object;
                string href =
                    Utils.GetObjectName(graph, formNode, HCTL.hasTarget)
                    ?? throw new Exception("mandatory form target not found");
                string? contentType = Utils.GetObjectName(graph, formNode, HCTL.forContentType);
                string? contentCoding = Utils.GetObjectName(graph, formNode, HCTL.forContentCoding);
                // security is not available in the parsed graph
                List<string>? scopes = Utils.GetObjectNames(graph, formNode, WoTSec.scopes);
                ExpectedResponse? expectedResponse = ReadExpectedResponse(formNode);
                List<AdditionalExpectedResponse>? additionalExpectedResponse =
                    ReadAdditionalExpectedResponse(formNode, contentType);
                string? subprotocol = Utils.GetObjectName(graph, formNode, HCTL.forSubProtocol);
                List<Op>? op = ReadOp(formNode);

                Form form = new Form(
                    href,
                    contentType,
                    contentCoding,
                    // security,
                    scopes,
                    expectedResponse,
                    additionalExpectedResponse,
                    subprotocol,
                    op,
                    affordanceType,
                    readOnly,
                    writeOnly
                );

                forms.Add(form);
            }
            if (forms.Count == 0)
                return null;
            return forms;
        }

        ExpectedResponse? ReadExpectedResponse(INode formNode)
        {
            INode? responseNode = Utils.GetObjectNode(graph, formNode, HCTL.returns);
            if (responseNode == null)
                return null;
            string contentType =
                Utils.GetObjectName(graph, responseNode, HCTL.forContentType)
                ?? throw new Exception("mandatory expectedResponse content type not found");

            return new ExpectedResponse(contentType);
        }

        List<AdditionalExpectedResponse>? ReadAdditionalExpectedResponse(
            INode formNode,
            string? formContentType
        )
        {
            formContentType ??= "application/json";
            IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
                formNode,
                this.graph.CreateUriNode(new Uri(HCTL.additionalReturns))
            );
            if (ts.Count() == 0)
                return null;

            List<AdditionalExpectedResponse> expectedResponses =
                new List<AdditionalExpectedResponse>();
            foreach (Triple t in ts)
            {
                INode responseNode = t.Object;
                bool? success = Utils.GetObjectBoolean(graph, responseNode, HCTL.isSuccess);
                string? contentType = Utils.GetObjectName(graph, responseNode, HCTL.forContentType);
                // schema is not available in the parsed graph

                contentType ??= formContentType;
                expectedResponses.Add(new AdditionalExpectedResponse(success, contentType));
            }

            if (expectedResponses.Count == 0)
                return null;
            else
                return expectedResponses;
        }

        List<Op>? ReadOp(INode formNode)
        {
            IEnumerable<Triple> ts = graph.GetTriplesWithSubjectPredicate(
                formNode,
                this.graph.CreateUriNode(new Uri(HCTL.hasOperationType))
            );
            if (ts.Count() == 0)
                return null;

            List<Op> ops = new List<Op>();
            foreach (Triple t in ts)
            {
                string opNode = t.Object.ToString();
                int index = opNode.LastIndexOf('#');
                string opLowercase = opNode.Substring(index + 1).ToLower();
                Op op = (Op)Enum.Parse(typeof(Op), opLowercase);
                ops.Add(op);
            }

            if (ops.Count == 0)
                return null;
            else
                return ops;
        }
    }
}