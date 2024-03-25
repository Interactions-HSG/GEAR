using System.Collections.Generic;

public class ArraySchema : DataSchema
{
    public readonly List<DataSchema>? items;
    public readonly int? minItems;
    public readonly int? maxItems;

    public ArraySchema(
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
        List<DataSchema>? items,
        int? minItems,
        int? maxItems,
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
        this.items = items;
        this.minItems = minItems;
        this.maxItems = maxItems;
    }

    public class Builder : Builder<ArraySchema, Builder>
    {
        List<DataSchema>? items;
        int? minItems;
        int? maxItems;
        readonly bool propertyAffordance;

        public Builder(bool? propertyAffordance)
        {
            this.propertyAffordance = propertyAffordance ?? false;
        }

        public Builder AddItems(List<DataSchema> items)
        {
            this.items = items;
            return this;
        }

        public Builder AddMinItems(int minItems)
        {
            this.minItems = minItems;
            return this;
        }

        public Builder AddMaxItems(int maxItems)
        {
            this.maxItems = maxItems;
            return this;
        }

        public override ArraySchema Build()
        {
            return new ArraySchema(
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
                items,
                minItems,
                maxItems,
                propertyAffordance
            );
        }
    }
}
