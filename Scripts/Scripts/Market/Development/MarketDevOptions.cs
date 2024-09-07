using Match3;
using Match3.Development.Base.DevelopmentConsole;

[DevOptionGroup(groupName: "Market", priority: 28)]
public class MarketDevOptions : DevelopmentOptionsDefinition
{
    [DevOption(commandName: "Force Fail PS Invalid Auth Token")]
    public static void ForceFailPlayStoreInvalidAuthorizationToken()
    {
        NormalizePlayStoreMarket();
        PlayStoreMarket.FORCE_FAIL_INVALID_AUTHORIZATION_TOKEN = true;   
    }

    [DevOption(commandName: "Force Fail PS Invalid Market")]
    public static void ForceFailPlayStoreInvalidMarket()
    {
        NormalizePlayStoreMarket();
        PlayStoreMarket.FORCE_FAIL_INVALID_MARKET = true;
    }

    [DevOption(commandName: "Force Fail PS Invalid Package Name")]
    public static void ForceFailPlayStoreInvalidPackageName()
    {
        NormalizePlayStoreMarket();
        PlayStoreMarket.FORCE_FAIL_INVALID_PACKAGE_NAME = true;
    }

    [DevOption(commandName: "Force Fail PS Invalid SKU")]
    public static void ForceFailPlayStoreInvalidSku()
    {
        NormalizePlayStoreMarket();
        PlayStoreMarket.FORCE_FAIL_INVALID_SKU = true;
    }

    [DevOption(commandName: "Force Fail PS Invalid Transaction Id")]
    public static void ForceFailPlayStoreInvalidTransactionId()
    {
        NormalizePlayStoreMarket();
        PlayStoreMarket.FORCE_FAIL_INVALID_TANSACTION_ID = true;
    }

    [DevOption(commandName: "Force Verify PS Purchase")]
    public static void ForceVerifyPlayStorePurchase()
    {
        KeepAliveHelper.VerifyGT_Debug(
            sku: "match3_special_package_a_0",
            transactionId: "chdpobgjlncmokpeohdnajkg.AO-J1OxOQuXZbRMKw2nvP9xNdQgsEqn_btST-RwGeqWOURUVIiu6gH3wgH19v6MlU9wEF3-cvOGZx-3lj-fx-KBbNUPksxgdQg"
        );
    }

    [DevOption(commandName: "Normalize PS Market")]
    public static void NormalizePlayStoreMarket()
    {
        PlayStoreMarket.FORCE_FAIL_INVALID_AUTHORIZATION_TOKEN = false;
        PlayStoreMarket.FORCE_FAIL_INVALID_MARKET = false;
        PlayStoreMarket.FORCE_FAIL_INVALID_PACKAGE_NAME = false;
        PlayStoreMarket.FORCE_FAIL_INVALID_SKU = false;
        PlayStoreMarket.FORCE_FAIL_INVALID_TANSACTION_ID = false;
    }
}
