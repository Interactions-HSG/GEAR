using System;

public class WoTSec
{
    public static String PREFIX = "https://www.w3.org/2019/wot/security#";

    /* Classes */
    public static String NoSecurityScheme = PREFIX + "NoSecurityScheme";
    public static String APIKeySecurityScheme = PREFIX + "APIKeySecurityScheme";
    public static String BasicSecurityScheme = PREFIX + "BasicSecurityScheme";
    public static String DigestSecurityScheme = PREFIX + "DigestSecurityScheme";
    public static String BearerSecurityScheme = PREFIX + "BearerSecurityScheme";
    public static String PSKSecurityScheme = PREFIX + "PSKSecurityScheme";
    public static String OAuth2SecurityScheme = PREFIX + "OAuth2SecurityScheme";

    /* Object properties */
    public static String authorization = PREFIX + "authorization";
    public static String token = PREFIX + "token";
    public static String refresh = PREFIX + "refresh";

    /* Datatype properties */
    public static String in_ = PREFIX + "in";
    public static String name = PREFIX + "name";
    public static String qop = PREFIX + "qop";
    public static String alg = PREFIX + "alg";
    public static String format = PREFIX + "format";
    public static String identity = PREFIX + "identity";
    public static String flow = PREFIX + "flow";
    public static String scopes = PREFIX + "scopes";

    public static String proxy = PREFIX + "proxy";
    public static String AutoSecurityScheme = PREFIX + "AutoSecurityScheme";
    public static String ComboSecurityScheme = PREFIX + "ComboSecurityScheme";
    public static String oneOf = PREFIX + "oneOf";
    public static String allOf = PREFIX + "allOf";

    // public static IRI createIRI(String fragment) {
    //   return SimpleValueFactory.getInstance().createIRI(PREFIX + fragment);
    // }

    WoTSec() { }
}
