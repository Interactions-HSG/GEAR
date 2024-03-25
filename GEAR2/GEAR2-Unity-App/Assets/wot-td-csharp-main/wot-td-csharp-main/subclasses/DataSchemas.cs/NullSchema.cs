public class NullSchema : DataSchema
{
    public NullSchema()
        : base(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            DataSchemaType.@null,
            false
        ) { }

    public class Builder
    {
        public NullSchema Build()
        {
            return new NullSchema();
        }
    }
}
