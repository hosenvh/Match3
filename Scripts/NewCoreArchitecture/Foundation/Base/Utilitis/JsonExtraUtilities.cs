using NiceJson;


namespace KitchenParadise.Utiltiy.Base
{
    public static class JsonExtraUtilities
    {
        public static void OverrideJsonWith(ref JsonObject originalJson, JsonObject overrideJson)
        {
            foreach (string overrideJsonNodeKey in overrideJson.Keys)
            {
                if (originalJson.ContainsKey(overrideJsonNodeKey))
                    originalJson[overrideJsonNodeKey] = overrideJson[overrideJsonNodeKey];
            }
        }
    }
}