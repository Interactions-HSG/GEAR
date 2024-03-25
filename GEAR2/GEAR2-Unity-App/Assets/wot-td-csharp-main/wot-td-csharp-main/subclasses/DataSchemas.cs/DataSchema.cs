using System.Collections.Generic;

public abstract class DataSchema
{
    public readonly string? title;
    // public readonly Dictionary<string, string>? Titles;
    public readonly string? description;
    // public readonly Dictionary<string, string>? Descriptions;
    // public readonly dynamic? Const;
    public readonly dynamic? default_;
    public readonly string? unit;
    public List<DataSchema>? oneOf;
    public List<dynamic>? enum_;
    public readonly bool? readOnly;
    public readonly bool? writeOnly;
    public readonly string? format;
    public readonly DataSchemaType? dataType;

    public DataSchema(
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
        bool? propertyAffordance
    )
    {
        this.title = title;
        // this.Titles = titles;
        this.description = description;
        // this.Descriptions = descriptions;
        // this.Const = const_;
        this.default_ = default_;
        this.unit = unit;
        this.oneOf = oneOf;
        this.enum_ = enum_;
        if (readOnly == null && propertyAffordance == true)
            readOnly = false;
        this.readOnly = readOnly;
        if (writeOnly == null && propertyAffordance == true)
            writeOnly = false;
        this.writeOnly = writeOnly;
        this.format = format;
        this.dataType = dataType;
    }

    public abstract class Builder<T, S>
        where T : DataSchema
        where S : Builder<T, S>
    {
        protected string? title;
        protected Dictionary<string, string>? titles;
        protected string? description;
        protected Dictionary<string, string>? descriptions;
        protected dynamic? const_;
        protected dynamic? default_;
        protected string? unit;
        protected List<DataSchema>? oneOf;
        protected List<dynamic>? enum_;
        protected bool? readOnly;
        protected bool? writeOnly;
        protected string? format;
        protected DataSchemaType? dataType;

        public Builder<T, S> AddTitle(string title)
        {
            this.title = title;
            return this;
        }

        public Builder<T, S> AddDescription(string description)
        {
            this.description = description;
            return this;
        }

        public Builder<T, S> AddDefault(dynamic default_)
        {
            this.default_ = default_;
            return this;
        }

        public Builder<T, S> AddUnit(string unit)
        {
            this.unit = unit;
            return this;
        }

        public Builder<T, S> AddOneOf(DataSchema oneOf)
        {
            if (this.oneOf == null)
                this.oneOf = new List<DataSchema>();
            this.oneOf.Add(oneOf);
            return this;
        }

        public Builder<T, S> AddOneOfs(List<DataSchema> oneOfs)
        {
            this.oneOf ??= new List<DataSchema>();
            this.oneOf.AddRange(oneOfs);
            return this;
        }

        public Builder<T, S> AddEnum(List<dynamic> enum_)
        {
            this.enum_ = enum_;
            return this;
        }

        public Builder<T, S> AddReadOnly(bool readOnly)
        {
            this.readOnly = readOnly;
            return this;
        }

        public Builder<T, S> AddWriteOnly(bool writeOnly)
        {
            this.writeOnly = writeOnly;
            return this;
        }

        public Builder<T, S> AddFormat(string format)
        {
            this.format = format;
            return this;
        }

        public Builder<T, S> AddDataType(DataSchemaType dataType)
        {
            this.dataType = dataType;
            return this;
        }

        public abstract T Build();
    }
}

public class MyDataSchema : DataSchema
{
    public MyDataSchema(
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
        bool propertyAffordance = false
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
    { }

    public class Builder : Builder<MyDataSchema, Builder>
    {
        readonly bool propertyAffordance;

        public Builder(bool? propertyAffordance)
        {
            this.propertyAffordance = propertyAffordance ?? false;
        }

        public override MyDataSchema Build()
        {
            return new MyDataSchema(
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
                propertyAffordance
            );
        }
    }
}
