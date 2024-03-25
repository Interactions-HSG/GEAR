using System.Collections.Generic;

public class BearerSecurityScheme : SecurityScheme
{
    public readonly string? authorization;
    public readonly string? name;
    public readonly string? alg;
    public readonly string? format;
    public readonly string? in_;

    public BearerSecurityScheme(
        string? description,
        Dictionary<string, string>? descriptions,
        string? proxy,
        string scheme,
        string? authorization,
        string? name,
        string? alg,
        string? format,
        string? in_
    )
        : base(description, descriptions, proxy, scheme)
    {
        this.authorization = authorization;
        this.name = name;
        if (alg == null)
            this.alg = "ES256";
        else
            this.alg = alg;
        if (format == null)
            this.format = "jwt";
        else
            this.format = format;
        if (in_ == null)
            this.in_ = "header";
        else
            this.in_ = in_;
    }

    public class Builder : Builder<BearerSecurityScheme, Builder>
    {
        string? authorization;
        string? name;
        string? alg;
        string? format;
        string? in_;

        public Builder(string scheme)
            : base(scheme)
        { }

        public Builder SetAuthorization(string authorization)
        {
            this.authorization = authorization;
            return this;
        }

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

        public Builder SetAlg(string alg)
        {
            this.alg = alg;
            return this;
        }

        public Builder SetFormat(string format)
        {
            this.format = format;
            return this;
        }

        public override BearerSecurityScheme Build()
        {
            return new BearerSecurityScheme(
                this.description,
                this.descriptions,
                this.proxy,
                this.scheme,
                this.authorization,
                this.name,
                this.alg,
                this.format,
                this.in_
            );
        }
    }
}
