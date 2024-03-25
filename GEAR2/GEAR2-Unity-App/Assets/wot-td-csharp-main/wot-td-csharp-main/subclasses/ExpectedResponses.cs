public class ExpectedResponse
{
    public string contentType;

    public ExpectedResponse(string contentType)
    {
        this.contentType = contentType;
    }
}

public class AdditionalExpectedResponse
{
    public bool? success;
    public string? contentType;

    // public string? schema;

    public AdditionalExpectedResponse(
        bool? success,
        string contentType
    // string? schema
    )
    {
        if (success == null)
            this.success = false;
        else
            this.success = success;
        this.contentType = contentType;
        // this.schema = schema;
    }
}
