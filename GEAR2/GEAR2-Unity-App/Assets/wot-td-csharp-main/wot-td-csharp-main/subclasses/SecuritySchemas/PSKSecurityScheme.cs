using System.Collections.Generic;

public class PSKSecurityScheme : SecurityScheme
{
    public readonly string? identity;

    public PSKSecurityScheme(
        string? description,
        Dictionary<string, string>? descriptions,
        string? proxy,
        string scheme,
        string? identity
    )
        : base(description, descriptions, proxy, scheme)
    {
        this.identity = identity;
    }

    public class Builder : Builder<PSKSecurityScheme, Builder>
    {
        string? identity;

        public Builder(string scheme)
            : base(scheme) { }

        public Builder SetIdentity(string identity)
        {
            this.identity = identity;
            return this;
        }

        public override PSKSecurityScheme Build()
        {
            return new PSKSecurityScheme(
                this.description,
                this.descriptions,
                this.proxy,
                this.scheme,
                this.identity
            );
        }
    }
}
