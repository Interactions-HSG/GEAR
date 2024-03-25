using System.Collections.Generic;

public abstract class InteractionAffordance
{
    public readonly string? title;

    public readonly Dictionary<string, string>? titles;

    public readonly string? description;

    public readonly Dictionary<string, string>? descriptions;

    public readonly List<Form> forms;

    public readonly Dictionary<string, DataSchema>? uriVariables;

    public InteractionAffordance(
        string? title,
        Dictionary<string, string>? titles,
        string? description,
        Dictionary<string, string>? descriptions,
        List<Form> forms,
        Dictionary<string, DataSchema>? uriVariables
    )
    {
        this.title = title;
        this.titles = titles;
        this.description = description;
        this.descriptions = descriptions;
        this.forms = forms;
        this.uriVariables = uriVariables;
    }

    public abstract class Builder<T, S>
        where T : InteractionAffordance
        where S : Builder<T, S>
    {
        protected string? type;
        protected string? title;
        protected Dictionary<string, string>? titles;
        protected string? description;
        protected Dictionary<string, string>? descriptions;
        protected List<Form> forms;
        protected Dictionary<string, DataSchema>? uriVariables;

        public Builder(List<Form> forms)
        {
            this.forms = forms;
            this.titles = new Dictionary<string, string>();
            this.descriptions = new Dictionary<string, string>();
            this.uriVariables = new Dictionary<string, DataSchema>();
        }

        public Builder<T, S> AddTitle(string title)
        {
            this.title = title;
            return this;
        }

        public Builder<T, S> AddDescription(string description)
        {
            this.description = description;
            return this;
        }

        public Builder<T, S> AddAllUriVarirables(Dictionary<string, DataSchema> uriVariables)
        {
            this.uriVariables = uriVariables;
            return this;
        }

        public abstract T build();
    }
}
