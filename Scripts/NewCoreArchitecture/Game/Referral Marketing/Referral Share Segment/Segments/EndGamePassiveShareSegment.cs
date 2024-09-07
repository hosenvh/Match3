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
    
    [CreateAssetMenu(menuName = "Match3/ReferralCenter/EndGamePassiveShareSegment")]
    public class EndGamePassiveShareSegment : ShareSegment
    {
        public int shareOfferIntervalCount;
        private int winningEndGameCount = 0;
        
        protected override bool IsInternalConditionsSatisfied()
        {
            return ServiceLocator.Find<GameTransitionManager>().IsInCampaign() && winningEndGameCount > shareOfferIntervalCount;
        }

        protected override void ResetInternalState()
        {
            winningEndGameCount = 0;
        }

        public override void UpdateInternalState(GameEvent gameEvent)
        {
            if (gameEvent is LevelEndedEvent levelEndedEvent &&
                ServiceLocator.Find<GameTransitionManager>().IsInCampaign())
            {
                if (levelEndedEvent.result == LevelResult.Win)
                {
                    winningEndGameCount++;
                }
            }
        }
    }
    
}


