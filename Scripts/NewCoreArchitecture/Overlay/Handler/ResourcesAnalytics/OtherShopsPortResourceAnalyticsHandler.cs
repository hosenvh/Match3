using DynamicSpecialOfferSpace;
using Match3.Foundation.Base.EventManagement;
using Match3.Game.PiggyBank;
using Match3.Game.ShopManagement;
using SeganX;
using DynamicSpecialOfferPurchaseResultGivingFinishedEvent = DynamicSpecialOfferSpace.DynamicSpecialOfferPurchaseResultGivingFinishedEvent;
using static GameAnalyticsDataProvider;
using Match3.LiveOps.SpecialOffer.Game;

namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class OtherShopsPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        private static class OtherShopsType
        {
            public const string KeyShop = "KeyShop_" + DEFAULT_ITEM_ID + "_";
            public const string SpecialOffer = "SpecialOffer_";
            public const string PiggyBank = "PiggyBank_" + DEFAULT_ITEM_ID + "_";
        }

        private static class SpecialOfferType
        {
            public const string Dynamic = "Dynamic_";
            public const string Event = "Event_";
        }

        public OtherShopsPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(itemType: ResourcesItemType.OtherShops, actions)
        {
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
        }

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case KeyShopPurchaseResultGivingStartedEvent data:
                    OpenPort(itemId: GetKeyShopItemId(data.package));
                    break;
                case KeyShopPurchaseResultGivingFinishedEvent data:
                    ClosePort(itemId: GetKeyShopItemId(data.package));
                    break;

                case DynamicSpecialOfferPurchaseResultGivingStartedEvent data:
                    OpenPort(itemId: GetDynamicSpecialOfferItemId(data.package));
                    break;
                case DynamicSpecialOfferPurchaseResultGivingFinishedEvent data:
                    ClosePort(itemId: GetDynamicSpecialOfferItemId(data.package));
                    break;

                case EventSpecialOfferPurchaseResultGivingStartedEvent data:
                    OpenPort(itemId: GetEventSpecialOfferItemId(data.package));
                    break;
                case EventSpecialOfferPurchaseResultGivingFinishedEvent data:
                    ClosePort(itemId: GetEventSpecialOfferItemId(data.package));
                    break;

                case PiggyBankPurchaseResultGivingStartedEvent data:
                    OpenPort(itemId: GetPiggyBankItemId(data.package));
                    break;
                case PiggyBankPurchaseResultGivingFinishedEvent data:
                    ClosePort(itemId: GetPiggyBankItemId(data.package));
                    break;
            }
        }

        private string GetKeyShopItemId(HardCurrencyPackage purchasingPackage)
        {
            return $"{OtherShopsType.KeyShop}{purchasingPackage.GetCamelCaseSku()}";
        }

        private string GetDynamicSpecialOfferItemId(HardCurrencyPackage purchasingPackage)
        {
            return $"{OtherShopsType.SpecialOffer}{SpecialOfferType.Dynamic}{purchasingPackage.GetCamelCaseSku()}";
        }

        private string GetEventSpecialOfferItemId(HardCurrencyPackage purchasingPackage)
        {
            return $"{OtherShopsType.SpecialOffer}{SpecialOfferType.Event}{purchasingPackage.GetCamelCaseSku()}";
        }

        private string GetPiggyBankItemId(HardCurrencyPackage purchasingPackage)
        {
            return $"{OtherShopsType.PiggyBank}{purchasingPackage.GetCamelCaseSku()}";
        }
    }
}