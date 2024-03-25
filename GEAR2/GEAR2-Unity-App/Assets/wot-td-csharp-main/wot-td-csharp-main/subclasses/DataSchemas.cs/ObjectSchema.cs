using System.Collections.Generic;

public class ObjectSchema : DataSchema
{
    public readonly Dictionary<string, DataSchema>? properties;
    public readonly List<string>? required;

    public ObjectSchema(
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
        Dictionary<string, DataSchema>? properties,
        List<string>? required,
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
        this.properties = properties;
        this.required = required;
    }

    public class Builder : Builder<ObjectSchema, Builder>
    {
        Dictionary<string, DataSchema>? properties;
        List<string>? required;
        readonly bool propertyAffordance;

        public Builder(bool? propertyAffordance)
        {
            this.propertyAffordance = propertyAffordance ?? false;
        }

        public Builder AddProperties(Dictionary<string, DataSchema> properties)
        {
            this.properties = properties;
            return this;
        }

        public Builder AddRequired(List<string> required)
        {
            this.required = required;
            return this;
        }

        public override ObjectSchema Build()
        {
            return new ObjectSchema(
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
                properties,
                required,
                propertyAffordance
            );
        }
    }
}
