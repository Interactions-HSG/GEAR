public class Link
{
    public readonly string href;
    public readonly string? type;
    public readonly string? rel;
    public readonly string? anchor;
    public readonly string? sizes;

    // string or Array of strings
    public readonly string? hreflang;

    public Link(
        string href,
        string? type,
        string? rel,
        string? anchor,
        string? sizes,
        string? hreflang
    )
    {
        this.href = href;
        this.type = type;
        this.rel = rel;
        this.anchor = anchor;
        this.sizes = sizes;
        this.hreflang = hreflang;
    }
}
