using System.Collections.Generic;

public class NumberSchema : DataSchema
{
    public readonly double? minimum;
    public readonly double? exclusiveMinimum;
    public readonly double? maximum;
    public readonly double? exclusiveMaximum;
    public readonly double? multipleOf;

    public NumberSchema(
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
        double? minimum,
        double? exclusiveMinimum,
        double? maximum,
        double? exclusiveMaximum,
        double? multipleOf,
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

    public class Builder : Builder<NumberSchema, Builder>
    {
        double? minimum;
        double? exclusiveMinimum;
        double? maximum;
        double? exclusiveMaximum;
        double? multipleOf;
        readonly bool propertyAffordance;

        public Builder(bool? propertyAffordance)
        {
            this.propertyAffordance = propertyAffordance ?? false;
        }

        public Builder AddMinimum(double minimum)
        {
            this.minimum = minimum;
            return this;
        }

        public Builder AddExclusiveMinimum(double exclusiveMinimum)
        {
            this.exclusiveMinimum = exclusiveMinimum;
            return this;
        }

        public Builder AddMaximum(double maximum)
        {
            this.maximum = maximum;
            return this;
        }

        public Builder AddExclusiveMaximum(double exclusiveMaximum)
        {
            this.exclusiveMaximum = exclusiveMaximum;
            return this;
        }

        public Builder AddMultipleOf(double multipleOf)
        {
            this.multipleOf = multipleOf;
            return this;
        }

        public override NumberSchema Build()
        {
            return new NumberSchema(
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
