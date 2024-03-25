using System.Collections.Generic;

public class StringSchema : DataSchema
{
    public readonly int? minLength;
    public readonly int? maxLength;
    public readonly string? pattern;
    public readonly string? contentEncoding;
    public readonly string? contentMediaType;

    public StringSchema(
        string? title,
        Dictionary<string, string>? titles,
        string? description,
        Dictionary<string, string>? descriptions,
        dynamic? const_,
        dynamic? default_,
        string? unit,
        List<DataSchema>? oneOf,
        List<dynamic>? enum_,
        bool? readOnly,
        bool? writeOnly,
        string? format,
        DataSchemaType? dataType,
        int? minLength,
        int? maxLength,
        string? pattern,
        string? contentEncoding,
        string? contentMediaType,
        bool? propertyAffordance
    )
        : base(
            title,
            titles,
            description,
            descriptions,
            (object?)const_,
            (object?)default_,
            unit,
            oneOf,
            enum_,
            readOnly,
            writeOnly,
            format,
            dataType,
            propertyAffordance
        )
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.pattern = pattern;
        this.contentEncoding = contentEncoding;
        this.contentMediaType = contentMediaType;
    }

    public class Builder : Builder<StringSchema, Builder>
    {
        int? minLength;
        int? maxLength;
        string? pattern;
        string? contentEncoding;
        string? contentMediaType;

        readonly bool propertyAffordance;

        public Builder(bool? propertyAffordance)
        {
            this.propertyAffordance = propertyAffordance ?? false;
        }

        public Builder AddMinLength(int minLength)
        {
            this.minLength = minLength;
            return this;
        }

        public Builder AddMaxLength(int maxLength)
        {
            this.maxLength = maxLength;
            return this;
        }

        public Builder AddPattern(string pattern)
        {
            this.pattern = pattern;
            return this;
        }

        public Builder AddContentEncoding(string contentEncoding)
        {
            this.contentEncoding = contentEncoding;
            return this;
        }

        public Builder AddContentMediaType(string contentMediaType)
        {
            this.contentMediaType = contentMediaType;
            return this;
        }

        public override StringSchema Build()
        {
            return new StringSchema(
                title,
                titles,
                description,
                descriptions,
                const_,
                default_,
                unit,
                oneOf,
                enum_,
                readOnly,
                writeOnly,
                format,
                dataType,
                minLength,
                maxLength,
                pattern,
                contentEncoding,
                contentMediaType,
                propertyAffordance
            );
        }
    }
}
