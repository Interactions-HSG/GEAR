using System;

public class HCTL
{
    public static String PREFIX = "https://www.w3.org/2019/wot/hypermedia#";

    public static String hasTarget = PREFIX + "hasTarget";
    public static String hasOperationType = PREFIX + "hasOperationType";

    public static String forContentType = PREFIX + "forContentType";
    public static String forSubProtocol = PREFIX + "forSubProtocol";
    public static String hasAnchor = PREFIX + "hasAnchor";
    public static String hasHreflang = PREFIX + "hasHreflang";
    public static String hasRelationType = PREFIX + "hasRelationType";
    public static String hasSizes = PREFIX + "hasSizes";
    public static String hintsAtMediaType = PREFIX + "hintsAtMediaType";
    public static String forContentCoding = PREFIX + "forContentCoding";
    public static String returns = PREFIX + "returns";
    public static String additionalReturns = PREFIX + "additionalReturns";
    public static String isSuccess = PREFIX + "isSuccess";

    // public static IRI createIRI(String fragment) {
    //   return SimpleValueFactory.getInstance().createIRI(PREFIX + fragment);
    // }

    HCTL() { }
}
