using System.Collections.Generic;

public class DigestSecurityScheme : SecurityScheme
{
    public readonly string? name;
    public readonly string? in_;
    public readonly string? qop;

    public DigestSecurityScheme(
        string? description,
        Dictionary<string, string>? descriptions,
        string? proxy,
        string scheme,
        string? name,
        string? in_,
        string? qop
    )
        : base(description, descriptions, proxy, scheme)
    {
        this.name = name;
        if (in_ == null)
            this.in_ = "header";
        else
            this.in_ = in_;
        if (qop == null)
            this.qop = "auth";
        else
            this.qop = qop;
    }

    public class Builder : Builder<DigestSecurityScheme, Builder>
    {
        string? name;
        string? in_;
        string? qop;

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

        public Builder SetQop(string qop)
        {
            this.qop = qop;
            return this;
        }

        public override DigestSecurityScheme Build()
        {
            return new DigestSecurityScheme(
                this.description,
                this.descriptions,
                this.proxy,
                this.scheme,
                this.name,
                this.in_,
                this.qop
            );
        }
    }
}
