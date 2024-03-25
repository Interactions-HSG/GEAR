using System.Collections.Generic;

public class IntegerSchema : DataSchema
{
    public readonly int? minimum;
    public readonly int? exclusiveMinimum;
    public readonly int? maximum;
    public readonly int? exclusiveMaximum;
    public readonly int? multipleOf;

    public IntegerSchema(
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
        int? minimum,
        int? exclusiveMinimum,
        int? maximum,
        int? exclusiveMaximum,
        int? multipleOf,
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
        this.minimum = minimum;
        this.exclusiveMinimum = exclusiveMinimum;
        this.maximum = maximum;
        this.exclusiveMaximum = exclusiveMaximum;
        this.multipleOf = multipleOf;
    }

    public class Builder : Builder<IntegerSchema, Builder>
    {
        int? minimum;
        int? exclusiveMinimum;
        int? maximum;
        int? exclusiveMaximum;
        int? multipleOf;
        readonly bool propertyAffordance;

        public Builder(bool? propertyAffordance)
        {
            this.propertyAffordance = propertyAffordance ?? false;
        }

        public Builder AddMinimum(int minimum)
        {
            this.minimum = minimum;
            return this;
        }

        public Builder AddExclusiveMinimum(int exclusiveMinimum)
        {
            this.exclusiveMinimum = exclusiveMinimum;
            return this;
        }

        public Builder AddMaximum(int maximum)
        {
            this.maximum = maximum;
            return this;
        }

        public Builder AddExclusiveMaximum(int exclusiveMaximum)
        {
            this.exclusiveMaximum = exclusiveMaximum;
            return this;
        }

        public Builder AddMultipleOf(int multipleOf)
        {
            this.multipleOf = multipleOf;
            return this;
        }

        public override IntegerSchema Build()
        {
            return new IntegerSchema(
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
                minimum,
                exclusiveMinimum,
                maximum,
                exclusiveMaximum,
                multipleOf,
                propertyAffordance
            );
        }
    }
}
