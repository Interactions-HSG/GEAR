using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

public class EventAffordance : InteractionAffordance
{
    public readonly string eventName;

    public readonly DataSchema? subscription;
    public readonly DataSchema? data;
    public readonly DataSchema? dataResponse;
    public readonly DataSchema? cancellation;

    public EventAffordance(
        string eventName,
        string? title,
        Dictionary<string, string>? titles,
        string? description,
        Dictionary<string, string>? descriptions,
        List<Form> forms,
        Dictionary<string, DataSchema>? uriVariables,
        DataSchema? subscription,
        DataSchema? data,
        DataSchema? dataResponse,
        DataSchema? cancellation
    )
        : base(title, titles, description, descriptions, forms, uriVariables)
    {
        this.eventName = eventName;
        this.subscription = subscription;
        this.data = data;
        this.dataResponse = dataResponse;
        this.cancellation = cancellation;
    }

    public async Task<string> GetEventValue()
    {
        Log.Information("fetching event value...");
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

    public class Builder : Builder<EventAffordance, Builder>
    {
        string name;
        DataSchema? subscription;
        DataSchema? data;
        DataSchema? dataResponse;
        DataSchema? cancellation;

        public Builder(string name, List<Form> forms)
            : base(forms)
        {
            this.name = name;
        }

        public Builder AddSubscription(DataSchema subscription)
        {
            this.subscription = subscription;
            return this;
        }

        public Builder AddData(DataSchema data)
        {
            this.data = data;
            return this;
        }

        public Builder AddDataResponse(DataSchema dataResponse)
        {
            this.dataResponse = dataResponse;
            return this;
        }

        public Builder AddCancellation(DataSchema cancellation)
        {
            this.cancellation = cancellation;
            return this;
        }

        public override EventAffordance build()
        {
            return new EventAffordance(
                this.name,
                this.title,
                this.titles,
                this.description,
                this.descriptions,
                this.forms,
                this.uriVariables,
                this.subscription,
                this.data,
                this.dataResponse,
                this.cancellation
            );
        }
    }
}
