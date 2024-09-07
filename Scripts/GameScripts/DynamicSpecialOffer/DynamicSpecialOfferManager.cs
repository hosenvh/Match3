using Medrick.Foundation.ShopManagement.Core;
using UnityEngine;
using static DynamicSpecialOfferSpace.DynamicSpecialOfferConsts;



namespace DynamicSpecialOfferSpace
{

    public struct CheckSpecialOfferResult
    {
        public bool HaveActiveOffer;
        public bool IsNewOffer;
        public bool IsFirstTimeOffer;
        public DynamicSpecialOfferPackage Offer;
    }
    
    public class DynamicSpecialOfferManager
    {

        // ---------------------------------------- Public Fields ---------------------------------------- \\

        // ---------------------------------------- Private Fields ---------------------------------------- \\  

        private readonly ShopCenter shopCenter;
        private readonly GameProfiler profiler;
        private readonly DynamicSpecialOfferConfig config;

        private bool isMenuUiNotNull;
        
        private bool haveActiveOffer = false;

        
        // ---------------------------------------- Properties ---------------------------------------- \\

        public DynamicSpecialOfferPackage SelectedOffer { private set; get; }

        public DynamicSpecialOfferPackage FirstTimeOffer => shopCenter.PackagesInCategoryAsArray<DynamicSpecialOfferPackage>(DYNAMIC_SPECIAL_OFFER_FIRST_OFFER_CATEGORY)[0];

        public DynamicSpecialOfferPackage[] RepeatingOffers => shopCenter.PackagesInCategoryAsArray<DynamicSpecialOfferPackage>(DYNAMIC_SPECIAL_OFFER_REPEATING_OFFERS_CATEGORY);

        // ================================================================================================ \\

        public DynamicSpecialOfferManager(DynamicSpecialOfferConfig config, ShopCenter shopCenter, GameProfiler profiler)
        {
            this.shopCenter = shopCenter;
            this.config = config;
            this.profiler = profiler;
            profiler.SubscribeToCoinChange(OnCoinChange);
            OnCoinChange(profiler.CoinCount);

            if (profiler.IsFirstOpenSet) return;
            profiler.IsFirstOpenSet = true;
            profiler.SetTimeFromFirstOpen();
        }

        public CheckSpecialOfferResult CheckForSpecialOffer()
        {
            SelectedOffer = null;

            CheckSpecialOfferResult result = new CheckSpecialOfferResult {IsNewOffer = true};

            if (IsAnyOfferActive())
            {
                result.IsNewOffer = false;
                
                if (profiler.PreviousDynamicSpecialOfferIndex < 0)
                    SelectedOffer = FirstTimeOffer;
                else
                    SelectedOffer = RepeatingOffers[profiler.PreviousDynamicSpecialOfferIndex];
            }
            else
            {
                SelectedOffer = TrySetNewOffer();
            }

            result.Offer = SelectedOffer;
            haveActiveOffer = SelectedOffer != null;
            result.HaveActiveOffer = haveActiveOffer;
            result.IsFirstTimeOffer = (haveActiveOffer && result.Offer == FirstTimeOffer);
            profiler.HaveDynamicSpecialOffer = haveActiveOffer;
            return result;
        }

        
        public DynamicSpecialOfferPackage ForceSpecialOffer(int offerIndex)
        {
            profiler.PreviousDynamicSpecialOfferIndex = offerIndex;
            
            profiler.HaveBoughtLastDynamicSpecialOffer = false;
            profiler.SetStartTimeOfDynamicSpecialOffer();
            
            haveActiveOffer = true;
            profiler.HaveDynamicSpecialOffer = true;
            return offerIndex == -1 ? FirstTimeOffer : RepeatingOffers[offerIndex];
        }
        

        private DynamicSpecialOfferPackage TrySetNewOffer()
        {
            if (!ResolveFirstWaitTimeCondition() || !ResolveTimePassedFromLastOfferCondition()) return null;
            
            if(ResolveNoPurchaseCondition() && ResolveLowCoinCondition())
            {
                profiler.PreviousDynamicSpecialOfferIndex = -1;
                profiler.HaveBoughtLastDynamicSpecialOffer = false;
                profiler.WasLowCoinCauseOfPreviousDynamicSpecialOffer = ResolveLowCoinCondition();
                profiler.SetStartTimeOfDynamicSpecialOffer();
                return FirstTimeOffer;
            }
            
            if (ResolveTimePassedFromLastPurchaseCondition() || ResolveLowCoinConditionAndLowCoinTime())
            {
                int selectedIndex = config.defaultOfferIndex;

                if (profiler.PreviousDynamicSpecialOfferIndex == -1 && profiler.BiggestPurchasePrice != -1)
                {
                    var biggestPurchase = profiler.BiggestPurchasePrice;
                    var diff = 999999;

                    for (int i = RepeatingOffers.Length - 1; i >= 0; --i)
                    {
                        var tempDiff = Mathf.Abs((int)RepeatingOffers[i].Price().Amount() - biggestPurchase);
                        if (tempDiff > diff) continue;
                        diff = tempDiff;
                        selectedIndex = i;
                    }
                }
                else if(profiler.PreviousDynamicSpecialOfferIndex >= 0)
                {
                    if (profiler.HaveBoughtLastDynamicSpecialOffer)
                        selectedIndex = ++profiler.PreviousDynamicSpecialOfferIndex;
                    else if(profiler.WasLowCoinCauseOfPreviousDynamicSpecialOffer)
                        selectedIndex = --profiler.PreviousDynamicSpecialOfferIndex;
                    else
                        selectedIndex = profiler.PreviousDynamicSpecialOfferIndex;
                    
                    selectedIndex = Mathf.Clamp(selectedIndex, 0, RepeatingOffers.Length - 1);
                }

                profiler.PreviousDynamicSpecialOfferIndex = selectedIndex;
                profiler.HaveBoughtLastDynamicSpecialOffer = false;
                profiler.WasLowCoinCauseOfPreviousDynamicSpecialOffer = ResolveLowCoinConditionAndLowCoinTime();
                profiler.SetStartTimeOfDynamicSpecialOffer();
                return RepeatingOffers[selectedIndex];
            }

            return null;
        }

  
        #region Condition Resolvers

        private bool ResolveFirstWaitTimeCondition()
        {
            return profiler.TimeFromFirstOpen >= config.firstOfferWaitTime;
        }
        
        private bool ResolveTimePassedFromLastOfferCondition()
        {
            return (profiler.TimeFromLastDynamicSpecialOffer >= (config.offerShowTime + config.offerGapTime) ||
                    profiler.TimeFromLastDynamicSpecialOffer == 0);
        }

        private bool ResolveLowCoinCondition()
        {
            return profiler.CoinCount <= config.lowCoinThreshold;
        }

        private bool ResolveLowCoinConditionAndLowCoinTime()
        {
            return profiler.TimeFromLastLowCoin >= config.requiredTimePassFromLowCoin
                && ResolveLowCoinCondition();
        }

        private bool ResolveTimePassedFromLastPurchaseCondition()
        {
            return (profiler.TimeFromLastPurchase >= config.requiredTimePassFromLastPurchase);
        }

        private bool ResolveNoPurchaseCondition()
        {
            return profiler.PurchaseCount <= 0;
        }

        #endregion

        // public bool HaveActiveOffer()
        // {
        //     if (!haveActiveOffer) return false;
        //     var remainingTime = config.offerShowTime - (int) profiler.TimeFromLastDynamicSpecialOffer;
        //     haveActiveOffer = remainingTime > 0;
        //     return haveActiveOffer;
        // }
        
        public bool UpdateCurrentOfferTimer(out int remainingTime)
        {
            remainingTime = 0;
            if (!haveActiveOffer) return false;

            remainingTime = config.offerShowTime - (int) profiler.TimeFromLastDynamicSpecialOffer;
            haveActiveOffer = remainingTime >= 0;
            if (!haveActiveOffer) profiler.HaveDynamicSpecialOffer = false;
            return haveActiveOffer;
        }


        void OnCoinChange(int newAmount)
        {
            var lowCoinFlag = profiler.IsLowCoinFlagSet;
            if (newAmount <= config.lowCoinThreshold && !lowCoinFlag)
            {
                profiler.IsLowCoinFlagSet = true;
                profiler.SetTimeFromLastLowCoin();
            }
            else if (newAmount > config.lowCoinThreshold+2500 && lowCoinFlag)
                profiler.IsLowCoinFlagSet = false;
        }

        private bool IsAnyOfferActive()
        {
            if (!profiler.HaveDynamicSpecialOffer) return false;
            return profiler.TimeFromLastDynamicSpecialOffer < config.offerShowTime;
        }
        
        
        #region  Deprecated

//        private bool ResolveCondition(DynamicSpecialOfferCondition condition)
//        {
//            return condition.requiredCoin<=profiler.CoinCount 
//                   && condition.requiredLevel<=profiler.LastUnlockedLevel 
//                   && condition.requiredPurchaseCount<=profiler.PurchaseCount
//                   && (condition.requiredTimeSpentFromLastOffer<=profiler.TimeFromLastDynamicSpecialOffer || profiler.TimeFromLastDynamicSpecialOffer==0)
//                   && (condition.requiredTimeSpentFromLastPurchase<=profiler.TimeFromLastPurchase || profiler.TimeFromLastPurchase==0)
//                   && condition.requiredTotalPreviousPurchasePrice<=profiler.TotalPreviousPurchasePrice;
//        }

        #endregion

        #region Deprecated

        //        void StartOfferCountDownTimer()
//        {
//            StartCoroutine(nameof(CStartOfferCountDownTimer));
//        }
//
//        private IEnumerator CStartOfferCountDownTimer()
//        {
//            var wOneSecond = new WaitForSeconds(1);
//            var remainingTime = offerActiveTime - profiler.TimeFromLastSpecialOffer;
//            while (remainingTime>=0)
//            {
//                remainingTime = offerActiveTime - profiler.TimeFromLastSpecialOffer;
//                if(isMenuUiNotNull)
//                    menuUi.UpdateTimer(Utilities.GetFormatedTime((int)remainingTime));
//
//                yield return wOneSecond;
//            }
//            
//            if(isMenuUiNotNull) menuUi.DeactivateButton();
//            profiler.HaveDynamicSpecialOffer = false;
//        }

        #endregion
    }
}