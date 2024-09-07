using UnityEngine;


namespace DynamicSpecialOfferSpace
{
    [System.Serializable]
    public class DynamicSpecialOfferCondition // : Condition
    {
        public int  requiredLevel = 0;
        public int  requiredCoin  = 0;
        public int  requiredPurchaseCount = 0;
        public long requiredTimeSpentFromLastOffer = 0;
        public long requiredTimeSpentFromLastPurchase = 0;
        public int  requiredTotalPreviousPurchasePrice = 0;
        
//        public override bool ResolveCondition()
//        {
//            var profiler = Base.gameManager.profiler;
//            return requiredCoin<=profiler.CoinCount 
//                   && requiredLevel<=profiler.LastUnlockedLevel 
//                   && requiredPurchaseCount<=profiler.PurchaseCount
//                   && (requiredTimeSpentFromLastOffer<=profiler.TimeFromLastDynamicSpecialOffer || profiler.TimeFromLastDynamicSpecialOffer==0)
//                   && (requiredTimeSpentFromLastPurchase<=profiler.TimeFromLastPurchase || profiler.TimeFromLastPurchase==0)
//                   && requiredTotalPreviousPurchasePrice<=profiler.TotalPreviousPurchasePrice;
//        }
    }

}
