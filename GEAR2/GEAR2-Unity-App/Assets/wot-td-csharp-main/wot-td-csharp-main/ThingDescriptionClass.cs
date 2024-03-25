using System.Collections.Generic;

public class ThingDescription
{
    // anyURI or Array
    // public string Context { get; set; }
    public string? id;
    public string title;

    // public Dictionary<string, string>? Titles { get; set; }
    public string? description;

    // public Dictionary<string, string>? Descriptions { get; set; }
    public Version? version;

    public string? created;
    public string? modified;
    public string? support;

    public string? base_;
    public List<PropertyAffordance>? properties;
    public List<ActionAffordance>? actions;
    public List<EventAffordance>? events;
    public List<Link>? links;
    public List<Form>? forms;

    // string or Array of strings
    public List<string> security;
    public Dictionary<string, SecurityScheme> securityDefinitions;

    // string or array of strings
    public List<string>? Profile;
    public Dictionary<string, DataSchema>? schemaDefinitions;
    public Dictionary<string, DataSchema>? uriVariables;

    public ThingDescription(
        string title,
        List<string> security,
        Dictionary<string, SecurityScheme> securityDefinitions,
        List<PropertyAffordance>? properties,
        List<ActionAffordance>? actions,
        List<EventAffordance>? events
    )
    {
        this.title = title;
        this.security = security;
        this.securityDefinitions = securityDefinitions;
        this.properties = properties;
        this.actions = actions;
        this.events = events;
    }

    public List<string>? GetPropertyNames()
    {
        if (this.properties == null)
            return null;
        List<string> propertyNames = new List<string>();
        foreach (PropertyAffordance property in this.properties)
            propertyNames.Add(property.propertyName);
        return propertyNames;
    }

    public PropertyAffordance? GetPropertyByName(string propertyName)
    {
        if (this.properties == null)
            return null;
        foreach (PropertyAffordance property in this.properties)
            if (property.propertyName == propertyName)
                return property;
        return null;
    }

    public class Builder
    {
        string? id;
        readonly string title;

        // public Dictionary<string, string>? Titles { get; set; }
        string? description;

        // public Dictionary<string, string>? Descriptions { get; set; }
        Version? version;

        string? created;
        string? modified;
        string? support;

        string? base_;
        List<PropertyAffordance>? properties;
        List<ActionAffordance>? actions;
        List<EventAffordance>? events;
        List<Link>? links;
        List<Form>? forms;

        // string or Array of strings
        readonly List<string> security;
        Dictionary<string, SecurityScheme> securityDefinitions;

        // string or array of strings
        List<string>? Profile;
        Dictionary<string, DataSchema>? schemaDefinitions;
        Dictionary<string, DataSchema>? uriVariables;

        public Builder(
            string title,
            List<string> security,
            Dictionary<string, SecurityScheme> securityDefinitions
        )
        {
            this.title = title;
            this.security = security;
            this.securityDefinitions = securityDefinitions;
            this.properties = new List<PropertyAffordance>();
            this.actions = new List<ActionAffordance>();
            this.events = new List<EventAffordance>();
        }

        public Builder SetThingId(string id)
        {
            this.id = id;
            return this;
        }

        public Builder SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        public Builder SetVersion(Version version)
        {
            this.version = version;
            return this;
        }

        public Builder SetCreated(string created)
        {
            this.created = created;
            return this;
        }

        public Builder SetModified(string modified)
        {
            this.modified = modified;
            return this;
        }

        public Builder SetSupport(string support)
        {
            this.support = support;
            return this;
        }

        public Builder SetBase(string base_)
        {
            this.base_ = base_;
            return this;
        }

        public Builder AddThingProperty(PropertyAffordance property)
        {
            this.properties ??= new List<PropertyAffordance>();
            this.properties.Add(property);
            return this;
        }

        public Builder AddThingProperties(List<PropertyAffordance> properties)
        {
            this.properties ??= new List<PropertyAffordance>();
            this.properties.AddRange(properties);
            return this;
        }

        public Builder AddThingAction(ActionAffordance action)
        {
            this.actions ??= new List<ActionAffordance>();
            this.actions.Add(action);
            return this;
        }

        public Builder AddThingActions(List<ActionAffordance> actions)
        {
            this.actions ??= new List<ActionAffordance>();
            this.actions.AddRange(actions);
            return this;
        }

        public Builder AddThingEvent(EventAffordance event_)
        {
            this.events ??= new List<EventAffordance>();
            this.events.Add(event_);
            return this;
        }

        public Builder AddThingEvents(List<EventAffordance> events)
        {
            this.events ??= new List<EventAffordance>();
            this.events.AddRange(events);
            return this;
        }

        public Builder AddLink(Link link)
        {
            this.links ??= new List<Link>();
            this.links.Add(link);
            return this;
        }

        public Builder AddLinks(List<Link> links)
        {
            this.links ??= new List<Link>();
            this.links.AddRange(links);
            return this;
        }

        public Builder AddForm(Form form)
        {
            this.forms ??= new List<Form>();
            this.forms.Add(form);
            return this;
        }

        public Builder AddForms(List<Form> forms)
        {
            this.forms ??= new List<Form>();
            this.forms.AddRange(forms);
            return this;
        }

        public Builder AddSecurity(string security)
        {
            this.security.Add(security);
            return this;
        }

        public Builder AddSecurities(List<string> securities)
        {
            this.security.AddRange(securities);
            return this;
        }

        public Builder AddSecurityDefinition(string name, SecurityScheme securityDefinition)
        {
            this.securityDefinitions.Add(name, securityDefinition);
            return this;
        }

        public Builder AddSecurityDefinitions(
            Dictionary<string, SecurityScheme> securityDefinitions
        )
        {
            this.securityDefinitions = securityDefinitions;
            return this;
        }

        public Builder AddProfile(List<string> profiles)
        {
            this.Profile ??= new List<string>();
            this.Profile.AddRange(profiles);
            return this;
        }

        public Builder AddSchemaDefinition(string name, DataSchema schemaDefinition)
        {
            this.schemaDefinitions ??= new Dictionary<string, DataSchema>();
            this.schemaDefinitions.Add(name, schemaDefinition);
            return this;
        }

        public Builder AddSchemaDefinitions(Dictionary<string, DataSchema> schemaDefinitions)
        {
            this.schemaDefinitions ??= new Dictionary<string, DataSchema>();
            this.schemaDefinitions = schemaDefinitions;
            return this;
        }

        public Builder AddUriVariable(string name, DataSchema uriVariable)
        {
            this.uriVariables ??= new Dictionary<string, DataSchema>();
            this.uriVariables.Add(name, uriVariable);
            return this;
        }

        public Builder AddUriVariables(Dictionary<string, DataSchema> uriVariables)
        {
            this.uriVariables ??= new Dictionary<string, DataSchema>();
            this.uriVariables = uriVariables;
            return this;
        }

        public ThingDescription Build()
        {
            return new ThingDescription(
                this.title,
                this.security,
                this.securityDefinitions,
                this.properties,
                this.actions,
                this.events
            )
            {
                id = this.id,
                description = this.description,
                version = this.version,
                created = this.created,
                modified = this.modified,
                support = this.support,
                base_ = this.base_,
                links = this.links,
                forms = this.forms,
                Profile = this.Profile,
                schemaDefinitions = this.schemaDefinitions,
                uriVariables = this.uriVariables
            };
        }
    }
}

public class Version
{
    public string instance;
    public string? model;

    public Version(string instance, string? model)
    {
        this.instance = instance;
        this.model = model;
    }
}
