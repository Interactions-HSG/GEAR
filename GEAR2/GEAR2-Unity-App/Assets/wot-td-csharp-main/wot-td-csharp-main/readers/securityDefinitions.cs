using System;
using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Parsing;
using Serilog;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        Dictionary<string, SecurityScheme> ReadSecurityDefinitions()
        {
            Log.Information("reading security definitions...");
            Dictionary<string, SecurityScheme> securityDefinitions =
                new Dictionary<string, SecurityScheme>();
            foreach (
                Triple t in graph.GetTriplesWithSubjectPredicate(
                    baseNode,
                    graph.CreateUriNode(new Uri(TD.definesSecurityScheme))
                )
            )
            {
                INode securityDefinitionNode = t.Object;
                Log.Information("reading security definition with node: " + securityDefinitionNode);
                string securitySchemeType =
                    Utils
                        .GetObjectNode(graph, securityDefinitionNode, RdfSpecsHelper.RdfType)
                        ?.ToString() ?? throw new Exception("security scheme type not found");

                if (securitySchemeType == WoTSec.NoSecurityScheme)
                {
                    MySecurityScheme.Builder builder = new MySecurityScheme.Builder("nosec");
                    AddSecuritySchemeProperties(builder, securityDefinitionNode);
                    SecurityScheme securityScheme = builder.Build();
                    securityDefinitions.Add("nosec_sc", securityScheme);
                }
                else if (securitySchemeType == WoTSec.AutoSecurityScheme)
                {
                    MySecurityScheme.Builder builder = new MySecurityScheme.Builder("auto");
                    AddSecuritySchemeProperties(builder, securityDefinitionNode);
                    SecurityScheme securityScheme = builder.Build();
                    securityDefinitions.Add("auto_sc", securityScheme);
                }
                else if (securitySchemeType == WoTSec.ComboSecurityScheme)
                {
                    ComboSecurityScheme.Builder builder = new ComboSecurityScheme.Builder("combo");
                    AddSecuritySchemeProperties(builder, securityDefinitionNode);
                    AddComboSecuritySchemeProperties(builder, securityDefinitionNode);
                    ComboSecurityScheme securityScheme = builder.Build();
                    securityDefinitions.Add("combo_sc", securityScheme);
                }
                else if (securitySchemeType == WoTSec.BasicSecurityScheme)
                {
                    BasicSecurityScheme.Builder builder = new BasicSecurityScheme.Builder("basic");
                    AddSecuritySchemeProperties(builder, securityDefinitionNode);
                    AddBasicSecuritySchemeProperties(builder, securityDefinitionNode);
                    SecurityScheme securityScheme = builder.Build();
                    securityDefinitions.Add("basic_sc", securityScheme);
                }
                else if (securitySchemeType == WoTSec.DigestSecurityScheme)
                {
                    DigestSecurityScheme.Builder builder = new DigestSecurityScheme.Builder("digest");
                    AddSecuritySchemeProperties(builder, securityDefinitionNode);
                    AddDigestSecuritySchemeProperties(builder, securityDefinitionNode);
                    DigestSecurityScheme securityScheme = builder.Build();
                    securityDefinitions.Add("digest_sc", securityScheme);
                }
                else if (securitySchemeType == WoTSec.APIKeySecurityScheme)
                {
                    APIKeySecurityScheme.Builder builder = new APIKeySecurityScheme.Builder("apikey");
                    AddSecuritySchemeProperties(builder, securityDefinitionNode);
                    string? name = Utils
                        .GetObjectNode(graph, securityDefinitionNode, WoTSec.name)
                        ?.ToString();
                    string? in_ = Utils
                        .GetObjectNode(graph, securityDefinitionNode, WoTSec.in_)
                        ?.ToString();

                    if (name != null)
                        builder.SetName(name);
                    if (in_ != null)
                        builder.SetIn(in_);
                }
                else if (securitySchemeType == WoTSec.BearerSecurityScheme)
                {
                    BearerSecurityScheme.Builder builder = new BearerSecurityScheme.Builder("bearer");
                    AddSecuritySchemeProperties(builder, securityDefinitionNode);
                    AddBearerSecuritySchemeProperties(builder, securityDefinitionNode);
                    BearerSecurityScheme securityScheme = builder.Build();
                    securityDefinitions.Add("bearer_sc", securityScheme);
                }
                else if (securitySchemeType == WoTSec.PSKSecurityScheme)
                {
                    PSKSecurityScheme.Builder builder = new PSKSecurityScheme.Builder("psk");
                    AddSecuritySchemeProperties(builder, securityDefinitionNode);
                    AddPSKSecuritySchemeProperties(builder, securityDefinitionNode);
                    PSKSecurityScheme securityScheme = builder.Build();
                    securityDefinitions.Add("psk_sc", securityScheme);
                }
                else if (securitySchemeType == WoTSec.OAuth2SecurityScheme)
                {
                    string flow =
                        Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.flow)?.ToString()
                        ?? throw new Exception("mandatory flow for OAuth2SecurityScheme not found");
                    OAuth2SecurityScheme.Builder builder = new OAuth2SecurityScheme.Builder(
                        "oauth2",
                        flow
                    );
                    AddSecuritySchemeProperties(builder, securityDefinitionNode);
                    AddOAuth2SecuritySchemeProperties(builder, securityDefinitionNode);
                    OAuth2SecurityScheme securityScheme = builder.Build();
                    securityDefinitions.Add("oauth2_sc", securityScheme);
                }
            }

            Log.Information(securityDefinitions.Count + " security definitions found");

            if (securityDefinitions.Count == 0)
                throw new Exception("no mandatory security definitions found");
            return securityDefinitions;
        }

        void AddSecuritySchemeProperties<T, S>(
            SecurityScheme.Builder<T, S> builder,
            INode securityDefinitionNode
        )
            where T : SecurityScheme
            where S : SecurityScheme.Builder<T, S>
        {
            string? description = Utils
                .GetObjectName(graph, securityDefinitionNode, TD.description)
                ?.ToString();

            string? proxy = Utils
                .GetObjectNode(graph, securityDefinitionNode, WoTSec.proxy)
                ?.ToString();

            if (description != null)
                builder.SetDescription(description.ToString());
            if (proxy != null)
                builder.SetProxy(proxy.ToString());
        }

        void AddComboSecuritySchemeProperties(
            ComboSecurityScheme.Builder builder,
            INode securityDefinitionNode
        )
        {
            List<string> oneOf = new List<string>();
            List<string> allOf = new List<string>();
            foreach (
                Triple t in graph.GetTriplesWithSubjectPredicate(
                    securityDefinitionNode,
                    graph.CreateUriNode(new Uri(WoTSec.oneOf))
                )
            )
            {
                oneOf.Add(t.Object.ToString());
            }
            foreach (
                Triple t in graph.GetTriplesWithSubjectPredicate(
                    securityDefinitionNode,
                    graph.CreateUriNode(new Uri(WoTSec.allOf))
                )
            )
            {
                allOf.Add(t.Object.ToString());
            }
            if (oneOf.Count > 0)
                builder.SetOneOf(oneOf);
            if (allOf.Count > 0)
                builder.SetAllOf(allOf);
        }

        void AddBasicSecuritySchemeProperties(
            BasicSecurityScheme.Builder builder,
            INode securityDefinitionNode
        )
        {
            string? name = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.name);
            string? in_ = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.in_);

            if (name != null)
                builder.SetName(name);
            if (in_ != null)
                builder.SetIn(in_);
        }

        void AddDigestSecuritySchemeProperties(
            DigestSecurityScheme.Builder builder,
            INode securityDefinitionNode
        )
        {
            string? name = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.name)?.ToString();
            string? in_ = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.in_)?.ToString();
            string? qop = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.qop)?.ToString();
            if (name != null)
                builder.SetName(name);
            if (in_ != null)
                builder.SetIn(in_);
            if (qop != null)
                builder.SetQop(qop);
        }

        void AddApiKeySecuritySchemeProperties(
            APIKeySecurityScheme.Builder builder,
            INode securityDefinitionNode
        )
        {
            string? name = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.name)?.ToString();
            string? in_ = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.in_)?.ToString();

            if (name != null)
                builder.SetName(name);
            if (in_ != null)
                builder.SetIn(in_);
        }

        void AddBearerSecuritySchemeProperties(
            BearerSecurityScheme.Builder builder,
            INode securityDefinitionNode
        )
        {
            string? authorization = Utils
                .GetObjectName(graph, securityDefinitionNode, WoTSec.authorization)
                ?.ToString();
            string? name = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.name)?.ToString();
            string? alg = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.alg)?.ToString();
            string? format = Utils
                .GetObjectName(graph, securityDefinitionNode, WoTSec.format)
                ?.ToString();
            string? in_ = Utils.GetObjectName(graph, securityDefinitionNode, WoTSec.in_)?.ToString();

            if (authorization != null)
                builder.SetAuthorization(authorization);
            if (name != null)
                builder.SetName(name);
            if (alg != null)
                builder.SetAlg(alg);
            if (format != null)
                builder.SetFormat(format);
            if (in_ != null)
                builder.SetIn(in_);
        }

        void AddPSKSecuritySchemeProperties(
            PSKSecurityScheme.Builder builder,
            INode securityDefinitionNode
        )
        {
            string? identity = Utils
                .GetObjectName(graph, securityDefinitionNode, WoTSec.identity)
                ?.ToString();

            if (identity != null)
                builder.SetIdentity(identity);
        }

        void AddOAuth2SecuritySchemeProperties(
            OAuth2SecurityScheme.Builder builder,
            INode securityDefinitionNode
        )
        {
            string? authorization = Utils
                .GetObjectNode(graph, securityDefinitionNode, WoTSec.authorization)
                ?.ToString();
            string? token = Utils
                .GetObjectNode(graph, securityDefinitionNode, WoTSec.token)
                ?.ToString();
            string? refresh = Utils
                .GetObjectNode(graph, securityDefinitionNode, WoTSec.refresh)
                ?.ToString();
            List<string> scopes = new List<string>();
            foreach (
                Triple t in graph.GetTriplesWithSubjectPredicate(
                    securityDefinitionNode,
                    graph.CreateUriNode(new Uri(WoTSec.scopes))
                )
            )
            {
                scopes.Add(t.Object.ToString());
            }

            if (authorization != null)
                builder.SetAuthorization(authorization);
            if (token != null)
                builder.SetToken(token);
            if (refresh != null)
                builder.SetRefresh(refresh);
            if (scopes.Count > 0)
                builder.SetScopes(scopes);
        }
    }
}
