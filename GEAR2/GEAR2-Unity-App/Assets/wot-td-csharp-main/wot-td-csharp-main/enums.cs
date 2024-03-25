public enum TDFormat
{
    jsonld,
    ttl
}

public enum DataSchemaType
{
    @object,
    array,
    @string,
    number,
    integer,
    boolean,
    @null
}

public enum Op
{
    readproperty,
    writeproperty,
    observeproperty,
    unobserveproperty,
    invokeaction,
    queryaction,
    cancelaction,
    subscribeevent,
    unsubscribeevent,
    readallproperties,
    writeallproperties,
    readmultipleproperties,
    writemultipleproperties,
    observeallproperties,
    unobserveallproperties,
    subscribeallevents,
    unsubscribeallevents,
    queryallactions
}

public enum AffordanceType
{
    property,
    action,
    event_
}
