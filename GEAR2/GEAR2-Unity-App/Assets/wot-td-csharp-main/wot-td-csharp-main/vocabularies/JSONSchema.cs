using System;

public class JSONSchema
{
    public static String PREFIX = "https://www.w3.org/2019/wot/json-schema#";

    /* Classes */
    public static String ArraySchema = PREFIX + "ArraySchema";
    public static String BooleanSchema = PREFIX + "BooleanSchema";
    public static String DataSchema = PREFIX + "DataSchema";
    public static String IntegerSchema = PREFIX + "IntegerSchema";
    public static String NullSchema = PREFIX + "NullSchema";
    public static String NumberSchema = PREFIX + "NumberSchema";
    public static String ObjectSchema = PREFIX + "ObjectSchema";
    public static String StringSchema = PREFIX + "StringSchema";

    /* Object properties */
    public static String allOf = PREFIX + "allOf";
    public static String anyOf = PREFIX + "anyOf";
    public static String items = PREFIX + "items";
    public static String oneOf = PREFIX + "oneOf";
    public static String properties = PREFIX + "properties";

    /* Datatype properties */
    public static String constant = PREFIX + "constant";
    public static String enumeration = PREFIX + "enum";
    public static String format = PREFIX + "format";
    public static String maxItems = PREFIX + "maxItems";
    public static String maximum = PREFIX + "maximum";
    public static String minItems = PREFIX + "minItems";
    public static String minimum = PREFIX + "minimum";
    public static String propertyName = PREFIX + "propertyName";
    public static String readOnly = PREFIX + "readOnly";
    public static String required = PREFIX + "required";
    public static String writeOnly = PREFIX + "writeOnly";
    public static String contentMediaType = PREFIX + "contentMediaType";

    public static String default_ = PREFIX + "default";

    public static String exclusiveMaximum = PREFIX + "exclusiveMaximum";
    public static String exclusiveMinimum = PREFIX + "exclusiveMinimum";
    public static String multipleOf = PREFIX + "multipleOf";
    public static String minLength = PREFIX + "minLength";
    public static String maxLength = PREFIX + "maxLength";
    public static String pattern = PREFIX + "pattern";

    // public static IRI createIRI(String fragment)
    // {
    //     return SimpleValueFactory.getInstance().createIRI(PREFIX + fragment);
    // }

    JSONSchema() { }
}
