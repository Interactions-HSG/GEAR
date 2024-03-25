using System;
using System.Collections.Generic;

public class OAuth2SecurityScheme : SecurityScheme
{
    public readonly string? authorization;
    public readonly string? token;
    public readonly string? refresh;
    public readonly List<string>? scopes;
    public readonly string flow;

    public OAuth2SecurityScheme(
        string? description,
        Dictionary<string, string>? descriptions,
        string? proxy,
        string scheme,
        string? authorization,
        string? token,
        string? refresh,
        List<string>? scopes,
        string flow
    )
        : base(description, descriptions, proxy, scheme)
    {
        if (flow == "code" && (authorization == null || token == null))
        {
            throw new Exception("authorization and token are required for OAuth2 code flow");
        }
        else if (flow == "client" && token == null)
        {
            throw new Exception("token is required for OAuth2 client flow");
        }
        else if (flow == "clinet" && authorization != null)
        {
            throw new Exception("authorization is not allowed for OAuth2 client flow");
        }
        this.authorization = authorization;
        this.token = token;
        this.refresh = refresh;
        this.scopes = scopes;
        this.flow = flow;
    }

    public class Builder : Builder<OAuth2SecurityScheme, Builder>
    {
        string? authorization;
        string? token;
        string? refresh;
        List<string>? scopes;
        string flow;

        public Builder(string scheme, string flow)
            : base(scheme)
        {
            this.flow = flow;
        }

        public Builder SetAuthorization(string authorization)
        {
            this.authorization = authorization;
            return this;
        }

        public Builder SetToken(string token)
        {
            this.token = token;
            return this;
        }

        public Builder SetRefresh(string refresh)
        {
            this.refresh = refresh;
            return this;
        }

        public Builder SetScopes(List<string> scopes)
        {
            this.scopes = scopes;
            return this;
        }

        public override OAuth2SecurityScheme Build()
        {
            return new OAuth2SecurityScheme(
                this.description,
                this.descriptions,
                this.proxy,
                this.scheme,
                this.authorization,
                this.token,
                this.refresh,
                this.scopes,
                this.flow
            );
        }
    }
}
