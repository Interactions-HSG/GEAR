using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

public class PropertyAffordance : InteractionAffordance
{
    public readonly string propertyName;
    public readonly bool? observable;
    public readonly DataSchema? dataSchema;

    public PropertyAffordance(
        string? title,
        Dictionary<string, string>? titles,
        string? description,
        Dictionary<string, string>? descriptions,
        List<Form> forms,
        Dictionary<string, DataSchema>? uriVariables,
        string propertyName,
        bool? observable,
        DataSchema? schema
    )
        : base(title, titles, description, descriptions, forms, uriVariables)
    {
        this.propertyName = propertyName;
        if (observable == null)
            this.observable = false;
        else
            this.observable = observable;
        this.dataSchema = schema;
    }

    public class Builder : Builder<PropertyAffordance, Builder>
    {
        readonly string name;
        bool? observable;
        DataSchema? schema;

        public Builder(string name, List<Form> forms)
            : base(forms)
        {
            this.name = name;
        }

        public Builder AddDataSchema(DataSchema? schema)
        {
            this.schema = schema;
            return this;
        }

        public Builder SetObservable(bool? observable)
        {
            this.observable = observable;
            return this;
        }

        public override PropertyAffordance build()
        {
            return new PropertyAffordance(
                this.title,
                this.titles,
                this.description,
                this.descriptions,
                this.forms,
                this.uriVariables,
                this.name,
                this.observable,
                this.schema
            );
        }
    }

    public async Task<string> GetPropertValue()
    {
        Log.Information("fetching property value...");
        if (this.dataSchema?.writeOnly == true)
            throw new Exception("The property is write-only");
        HttpClient client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync(this.forms[0].href);
            int statusCode = (int)response.StatusCode;
            Log.Information("status code: " + statusCode);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
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

    public async Task<string> SetPropertyValue(dynamic value)
    {
        Log.Information("setting property value...");
        if (this.dataSchema?.readOnly == true)
            throw new Exception("The property is read-only");
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
}
