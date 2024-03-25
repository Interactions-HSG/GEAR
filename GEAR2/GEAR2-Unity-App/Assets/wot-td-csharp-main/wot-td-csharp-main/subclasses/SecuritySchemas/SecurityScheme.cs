using System.Collections.Generic;

public abstract class SecurityScheme
{
    public readonly string? description;
    public readonly Dictionary<string, string>? descriptions;
    public readonly string? proxy;
    public readonly string scheme;

    public SecurityScheme(
        string? description,
        Dictionary<string, string>? descriptions,
        string? proxy,
        string scheme
    )
    {
        this.description = description;
        this.descriptions = descriptions;
        this.proxy = proxy;
        this.scheme = scheme;
    }

    public abstract class Builder<T, S>
        where T : SecurityScheme
        where S : Builder<T, S>
    {
        protected string? description;
        protected Dictionary<string, string>? descriptions;
        protected string? proxy;
        protected string scheme;

        public Builder(string scheme)
        {
            this.scheme = scheme;
        }

        public Builder<T, S> SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        public Builder<T, S> SetDescriptions(Dictionary<string, string> descriptions)
        {
            this.descriptions = descriptions;
            return this;
        }

        public Builder<T, S> SetProxy(string proxy)
        {
            this.proxy = proxy;
            return this;
        }

        public abstract T Build();
    }
}

public class MySecurityScheme : SecurityScheme
{
    public MySecurityScheme(
        string? description,
        Dictionary<string, string>? descriptions,
        string? proxy,
        string scheme
    )
        : base(description, descriptions, proxy, scheme) { }

    public class Builder : Builder<MySecurityScheme, Builder>
    {
        public Builder(string scheme)
            : base(scheme) { }

        public override MySecurityScheme Build()
        {
            return new MySecurityScheme(
                this.description,
                this.descriptions,
                this.proxy,
                this.scheme
            );
        }
    }
}
