using System.Collections.Generic;

public class BasicSecurityScheme : SecurityScheme
{
    public readonly string? name;
    public readonly string? in_;

    public BasicSecurityScheme(
        string? description,
        Dictionary<string, string>? descriptions,
        string? proxy,
        string scheme,
        string? name,
        string? in_
    )
        : base(description, descriptions, proxy, scheme)
    {
        this.name = name;
        if (in_ == null)
            this.in_ = "header";
        else
            this.in_ = in_;
    }

    public class Builder : Builder<BasicSecurityScheme, Builder>
    {
        string? name;
        string? in_;

        public Builder(string scheme)
            : base(scheme)
        { }

        public Builder SetName(string name)
        {
            this.name = name;
            return this;
        }

        public Builder SetIn(string in_)
        {
            this.in_ = in_;
            return this;
        }

        public override BasicSecurityScheme Build()
        {
            return new BasicSecurityScheme(
                this.description,
                this.descriptions,
                this.proxy,
                this.scheme,
                this.name,
                this.in_
            );
        }
    }
}
