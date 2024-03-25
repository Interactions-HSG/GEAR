using System;
using System.Collections.Generic;

public class Form
{
    public readonly string href;
    public readonly string? contentType;
    public readonly string? contentCoding;
    // string or Array of strings
    public readonly List<string>? security;
    // string or Array of strings
    public readonly List<string>? scopes;
    public readonly ExpectedResponse? response;
    public List<AdditionalExpectedResponse>? additionalResponses;
    public readonly string? subprotocol;
    // string or Array of strings
    public List<Op>? op;

    public Form(
        string href,
        string? contentType,
        string? contentCoding,
        // List<string>? security,
        List<string>? scopes,
        ExpectedResponse? response,
        List<AdditionalExpectedResponse>? additionalResponses,
        string? subprotocol,
        List<Op>? op,
        AffordanceType? affordanceType,
        bool? readOnly,
        bool? writeOnly
    )
    {
        this.href = href;
        if (contentType == null)
            this.contentType = "application/json";
        else
            this.contentType = contentType;
        this.contentCoding = contentCoding;
        // this.security = security;
        this.scopes = scopes;
        this.response = response;
        this.additionalResponses = additionalResponses;
        this.subprotocol = subprotocol;

        if (affordanceType is AffordanceType.property)
        {
            if (
                op == null
                || !(
                    op.Contains(Op.readproperty)
                    || op.Contains(Op.writeproperty)
                    || op.Contains(Op.observeproperty)
                    || op.Contains(Op.unobserveproperty)
                )
            )
                throw new Exception(
                    "Form.op must include at least one of the following for a PropertyAffordance: readproperty, writeproperty, observeproperty, unobserveproperty"
                );

            if (readOnly != null && readOnly.Value && writeOnly != null && writeOnly.Value)
                this.op = new List<Op> { Op.readproperty, Op.writeproperty };
            else if (readOnly != null && readOnly.Value)
                this.op = new List<Op> { Op.readproperty };
            else if (writeOnly != null && writeOnly.Value)
                this.op = new List<Op> { Op.writeproperty };
        }
        else if (affordanceType is AffordanceType.property)
            this.op = new List<Op> { Op.invokeaction };
        else if (affordanceType is AffordanceType.event_)
            this.op = new List<Op> { Op.subscribeevent, Op.unsubscribeevent };

        if (this.op != null && op != null)
            this.op.AddRange(op);
        else
            this.op = op;
    }
}
