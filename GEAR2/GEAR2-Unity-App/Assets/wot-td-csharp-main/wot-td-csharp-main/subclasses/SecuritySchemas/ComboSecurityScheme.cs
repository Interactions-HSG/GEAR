using System.Collections.Generic;

public class ComboSecurityScheme : SecurityScheme
{
    public readonly List<string> oneOf;

    public readonly List<string> allOf;

    public ComboSecurityScheme(
        string? description,
        Dictionary<string, string>? descriptions,
        string? proxy,
        string scheme,
        List<string> oneOf,
        List<string> allOf
    )
        : base(description, descriptions, proxy, scheme)
    {
        this.oneOf = oneOf;
        this.allOf = allOf;
    }

    public class Builder : Builder<ComboSecurityScheme, Builder>
    {
        List<string> oneOf;

        List<string> allOf;

        public Builder(string scheme)
            : base(scheme)
        {
            this.oneOf = new List<string>();
            this.allOf = new List<string>();
        }

        public Builder SetOneOf(List<string> oneOf)
        {
            this.oneOf = oneOf;
            return this;
        }

        public Builder SetAllOf(List<string> allOf)
        {
            this.allOf = allOf;
            return this;
        }

        public override ComboSecurityScheme Build()
        {
            return new ComboSecurityScheme(
                this.description,
                this.descriptions,
                this.proxy,
                this.scheme,
                this.oneOf,
                this.allOf
            );
        }
    }
}
