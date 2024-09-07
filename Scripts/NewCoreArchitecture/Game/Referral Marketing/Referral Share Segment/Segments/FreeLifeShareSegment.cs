using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.Segments
{

    [CreateAssetMenu(menuName = "Match3/ReferralCenter/FreeLifeShareSegment")]
    public class FreeLifeShareSegment : ShareSegment
    {
        protected override bool IsInternalConditionsSatisfied()
        {
            return true;
        }

        protected override void ResetInternalState()
        {
            // Nothing For Now
        }

        public override void UpdateInternalState(GameEvent gameEvent)
        {
            // Nothing For Now
        }
    }

}