using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Game.ReferralMarketing.Segments;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.Segments
{

    [CreateAssetMenu(menuName = "Match3/ReferralCenter/ShopActiveShareSegment")]
    public class ShopActiveShareSegment : ShareSegment
    {
        [Space(10)]
        public int lessCoin;

        private bool playerHasPurchased;
        
        protected override bool IsInternalConditionsSatisfied()
        {
            return Base.gameManager.profiler.CoinCount <= lessCoin && !playerHasPurchased;
        }

        protected override void ResetInternalState()
        {
            // Nothing
        }

        public override void UpdateInternalState(GameEvent gameEvent)
        {
            playerHasPurchased = gameEvent is PurchaseSuccessEvent;
        }
    }

}