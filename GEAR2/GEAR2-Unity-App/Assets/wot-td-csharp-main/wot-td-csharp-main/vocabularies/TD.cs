using System;

public class TD
{
    public static String PREFIX = "https://www.w3.org/2019/wot/td#";

    /* Classes */
    public static String Thing = PREFIX + "Thing";
    public static String ActionAffordance = PREFIX + "ActionAffordance";
    public static String PropertyAffordance = PREFIX + "PropertyAffordance";
    public static String EventAffordance = PREFIX + "EventAffordance";

    /* Object properties */
    public static String hasBase = PREFIX + "hasBase";
    public static String name = PREFIX + "name";
    public static String title = PREFIX + "title";
    public static String description = PREFIX + "description";

    public static String hasInteractionAffordance = PREFIX + "hasInteractionAffordance";
    public static String hasActionAffordance = PREFIX + "hasActionAffordance";
    public static String hasPropertyAffordance = PREFIX + "hasPropertyAffordance";
    public static String hasEventAffordance = PREFIX + "hasEventAffordance";

    public static String hasSecurityConfiguration = PREFIX + "hasSecurityConfiguration";

    public static String isObservable = PREFIX + "isObservable";

    public static String hasInputSchema = PREFIX + "hasInputSchema";
    public static String hasOutputSchema = PREFIX + "hasOutputSchema";

    public static String hasSubscriptionSchema = PREFIX + "hasSubscriptionSchema";
    public static String hasNotificationSchema = PREFIX + "hasNotificationSchema";
    public static String hasCancellationSchema = PREFIX + "hasCancellationSchema";

    public static String hasForm = PREFIX + "hasForm";

    public static String hasUriTemplateSchema = PREFIX + "hasUriTemplateSchema";

    /* Named individuals */
    public static String readProperty = PREFIX + "readProperty";
    public static String writeProperty = PREFIX + "writeProperty";
    public static String invokeAction = PREFIX + "invokeAction";
    public static String observeProperty = PREFIX + "observeProperty";
    public static String unobserveProperty = PREFIX + "unobserveProperty";
    public static String subscribeEvent = PREFIX + "subscribeEvent";
    public static String unsubscribeEvent = PREFIX + "unsubscribeEvent";

    public static String isSafe = PREFIX + "isSafe";
    public static String isIdempotent = PREFIX + "isIdempotent";
    public static String isSynchronous = PREFIX + "isSynchronous";
    public static String hasNotificationResponseSchema = PREFIX + "hasNotificationResponseSchema";

    public static String definesSecurityScheme = PREFIX + "definesSecurityScheme";

    public static String versionInfo = PREFIX + "versionInfo";
    public static String instance = PREFIX + "instance";
    public static String model = PREFIX + "model";
    public static String supportContact = PREFIX + "supportContact";
    public static String baseURI = PREFIX + "baseURI";
    public static String hasLink = PREFIX + "hasLink";
    public static String followsProfile = PREFIX + "followsProfile";
    public static String schemaDefinitions = PREFIX + "schemaDefinitions";
    public static String type = PREFIX + "type";

    // public static IRI createIRI(String fragment)
    // {
    //     return SimpleValueFactory.getInstance().createIRI(PREFIX + fragment);
    // }

    TD() { }
}
