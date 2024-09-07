using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.Segments
{

    [CreateAssetMenu(menuName = "Match3/ReferralCenter/ShopPassiveShareSegment")]
    public class ShopPassiveShareSegment : ShareSegment
    {
        protected override bool IsInternalConditionsSatisfied()
        {
            return true;
        }

        protected override void ResetInternalState()
        {
            // Nothing
        }

        public override void UpdateInternalState(GameEvent gameEvent)
        {
            // Nothing
        }
    }

    
}

