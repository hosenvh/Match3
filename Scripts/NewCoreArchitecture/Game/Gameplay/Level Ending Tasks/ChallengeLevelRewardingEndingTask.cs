using System;
using System.Linq;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;


namespace Match3.Presentation.Gameplay
{

    public class ChallengeLevelRewardingEndingTask : LevelEndingTask
    {
        public class ChallengeLevelRewardGivingStartedEvent : GameEvent {}
        public class ChallengeLevelRewardGivingFinishedEvent : GameEvent {}

        private BoardConfig boardConfig;
        
        public ChallengeLevelRewardingEndingTask(BoardConfig boardConfig)
        {
            this.boardConfig = boardConfig;
        }

        protected override void ExecuteLevelEndingTask(Action onComplete)
        {
            //var hudPopup = Base.gameManager.OpenPopup<Popup_FakeHud>();
            var rewards = boardConfig.challengeLevelConfig.GetRewards();
            ServiceLocator.Find<EventManager>().Propagate(new ChallengeLevelRewardGivingStartedEvent(), this);
            foreach (var reward in rewards)
            {
                reward.Apply();
            }
            ServiceLocator.Find<EventManager>().Propagate(new ChallengeLevelRewardGivingFinishedEvent(), this);
            onComplete.Invoke();
            
            // Base.gameManager.OpenPopup<Popup_ClaimReward_Better>()
            //     .Setup(rewards.ToArray(), hudPopup.GetHudPresentationController(),
            //         false).OnComplete(() =>
            //     {
            //         Base.gameManager.ClosePopup(hudPopup);
            //         onComplete();
            //     })
            //     .SetEndDelay(0.75f).StartPresentingRewards();
        }
    }
    
}


