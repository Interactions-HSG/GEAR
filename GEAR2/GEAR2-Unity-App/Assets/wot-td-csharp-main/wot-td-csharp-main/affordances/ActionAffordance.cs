using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

public class ActionAffordance : InteractionAffordance
{
    public readonly string actionName;
    public readonly DataSchema? input;
    public readonly DataSchema? output;
    public readonly bool? safe;
    public readonly bool? idempotent;
    public readonly bool? synchronous;

    public ActionAffordance(
        string actionName,
        string? title,
        Dictionary<string, string>? titles,
        string? description,
        Dictionary<string, string>? descriptions,
        List<Form> forms,
        Dictionary<string, DataSchema>? uriVariables,
        DataSchema? input,
        DataSchema? output,
        bool? safe,
        bool? idempotent,
        bool? synchronous
    )
        : base(title, titles, description, descriptions, forms, uriVariables)
    {
        this.actionName = actionName;
        this.input = input;
        this.output = output;
        if (safe == null)
            this.safe = false;
        else
            this.safe = safe;
        if (idempotent == null)
            this.idempotent = false;
        else
            this.idempotent = idempotent;
        this.synchronous = synchronous;
    }

    public async Task<string> SetActionValue(dynamic value)
    {
        Log.Information("setting action value");
        HttpClient client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.PutAsync(
                this.forms[0].href,
                new StringContent(value)
            );
            int statusCode = (int)response.StatusCode;
            Log.Information("status code: " + statusCode);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            else
            {
                throw new Exception(
                    "The request did not succeed with status code: " + response.StatusCode
                );
            }
        }
        catch (HttpRequestException e)
        {
            throw new Exception("The request failed: " + e.Message);
        }
    }

    public class Builder : Builder<ActionAffordance, Builder>
    {
        readonly string actionName;
        DataSchema? input;
        DataSchema? output;
        bool? safe;
        bool? idempotent;
        bool? synchronous;

        public Builder(string name, List<Form> forms)
            : base(forms)
        {
            this.actionName = name;
        }

        public Builder AddInput(DataSchema input)
        {
            this.input = input;
            return this;
        }

        public Builder AddOutput(DataSchema output)
        {
            this.output = output;
            return this;
        }

        public Builder SetSafe(bool safe)
        {
            this.safe = safe;
            return this;
        }

        public Builder SetIdempotent(bool idempotent)
        {
            this.idempotent = idempotent;
            return this;
        }

        public Builder SetSynchronous(bool synchronous)
        {
            this.synchronous = synchronous;
            return this;
        }

        public override ActionAffordance build()
        {
            return new ActionAffordance(
                this.actionName,
                this.title,
                this.titles,
                this.description,
                this.descriptions,
                this.forms,
                this.uriVariables,
                this.input,
                this.output,
                this.safe,
                this.idempotent,
                this.synchronous
            );
        }
    }
}
