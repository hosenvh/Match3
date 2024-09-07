using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.ReferralMarketing.Segments;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.Segments
{

    [CreateAssetMenu(menuName = "Match3/ReferralCenter/EndGameActiveShareSegment")]
    public class EndGameActiveShareSegment : ShareSegment
    {
        [Space(10)] 
        public int loseCountBeforeAvailability;

        private int loseCount;
        private bool loseTargetCountReached = false;
        
        protected override bool IsInternalConditionsSatisfied()
        {
            return loseTargetCountReached;
        }

        protected override void ResetInternalState()
        {
            loseCount = 0;
        }

        public override void UpdateInternalState(GameEvent gameEvent)
        {
            if (gameEvent is LevelEndedEvent levelEndedEvent &&
                ServiceLocator.Find<GameTransitionManager>().IsInCampaign())
            {
                if (levelEndedEvent.result == LevelResult.Lose)
                    loseCount++;
                else if (levelEndedEvent.result == LevelResult.Win)
                {
                    loseTargetCountReached = loseCount >= loseCountBeforeAvailability;
                    ResetInternalState();
                }
            }
            
        }
    }

}