using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using Match3.Game;
using Match3.Game.Gameplay;
using Match3.Game.TaskManagement;
using Match3.Presentation.Hook.Containers;
using Match3.Presentation.HUD;
using Match3.Presentation.Rewards;
using SeganX;
using UnityEngine;
using UnityEngine.Events;


namespace Match3.Presentation.Gameplay
{
    public class PopupWinOpenedEvent : GameEvent
    {
        public Popup_WinBase winPopup;

        public PopupWinOpenedEvent(Popup_WinBase winPopup)
        {
            this.winPopup = winPopup;
        }
    }

    public class PopupWinSetupEvent : GameEvent
    {
        public Popup_WinBase winPopup;

        public PopupWinSetupEvent(Popup_WinBase winPopup)
        {
            this.winPopup = winPopup;
        }
    }
    
    public abstract class Popup_WinBase : GameState
    {
        [SerializeField] protected HudPresentationController hudPresentationController;
        [SerializeField] protected HookContainer hookContainer;
        [SerializeField] protected ScalableGridLayout bottomRewardLayoutController;
        [SerializeField] protected Transform bottomRewardCreationPositionHolder;
        [SerializeField] protected GameObject blockerObject;
        public UnityEvent onSkip;

        protected GameplayController gameplayController;
        protected SequentialSortedTaskChain taskChain;
        protected RewardPresentationFactory rewardPresentationFactory;
        protected List<BaseRewardPresentation> bottomRewardPresentations = new List<BaseRewardPresentation>();

        protected int score;
        private bool canSkip;
        private Action onExit;

        public HudPresentationController HudPresentationController => hudPresentationController;
        public HookContainer HookContainer => hookContainer;


        public void Setup(GameplayController gameplayController, int score, Action onExit)
        {
            this.score = score;
            this.gameplayController = gameplayController;
            this.onExit = onExit;
            taskChain = new SequentialSortedTaskChain();
            rewardPresentationFactory = new RewardPresentationFactory();

            ServiceLocator.Find<EventManager>().Propagate(new PopupWinOpenedEvent(this), this);
            InternalSetup();
            ServiceLocator.Find<EventManager>().Propagate(new PopupWinSetupEvent(this), this);
            taskChain.Execute(SetCanSkip, delegate {  });
        }

        public abstract void InternalSetup();


        
        public void CreateRewardPresentationAtBottom(Reward reward, float movingDelay)
        {
            var rewardPresentation =
                new RewardPresentationFactory().GenerateRewardPresentation(RewardPresentationType.BottomValue, reward, transform);
            AddRewardPresentationAtBottom(rewardPresentation, movingDelay);
        }

        public void AddRewardPresentationAtBottom(BaseRewardPresentation rewardPresentation, float movingDelay)
        {
            TryUpdateDelta(rewardPresentation);
            rewardPresentation.transform.position = bottomRewardCreationPositionHolder.position;
            SendRewardPresentationToRewardsPlace(rewardPresentation, movingDelay);
        }

        private void TryUpdateDelta(BaseRewardPresentation rewardPresentation)
        {
            if (DoesRewardPresentationAlreadyExist() == false)
                return;

            if (rewardPresentation.PresentingReward is CoinReward coinReward)
                hudPresentationController.TryGetHudElement(HudType.Coin).counter.AddDelta(-coinReward.count);

            bool DoesRewardPresentationAlreadyExist()
            {
                BaseRewardPresentation existingRewardPresentation = TryFindBottomRewardPresentation(rewardPresentation.PresentingReward);
                return existingRewardPresentation != null;
            }
        }

        private void SendRewardPresentationToRewardsPlace(BaseRewardPresentation rewardPresentation, float movingDelay)
        {
            rewardPresentation.transform.SetParent(transform, true, true, true);

            var existingRewardPresenter = TryFindBottomRewardPresentation(rewardPresentation.PresentingReward);
            if (existingRewardPresenter != null)
            {
                rewardPresentation.FollowTarget(existingRewardPresenter.transform, 750, movingDelay, true,
                    () => { MergeRewardPresentations(existingRewardPresenter, rewardPresentation); });
            }
            else
            {
                var gridCell = bottomRewardLayoutController.GetNewCell();
                rewardPresentation.FollowTarget(gridCell, 750, movingDelay, true, () => { });
                bottomRewardPresentations.Add(rewardPresentation);
            }
        }

        private void MergeRewardPresentations(BaseRewardPresentation existingRewardPresentation,
            BaseRewardPresentation mergingRewardPresentation)
        {
            existingRewardPresentation.AddToPresentingAmount(mergingRewardPresentation.PresentingReward);
            Destroy(mergingRewardPresentation.gameObject);
        }

        public BaseRewardPresentation TryFindBottomRewardPresentation(Reward forReward)
        {
            foreach (var bottomRewardPresentation in bottomRewardPresentations)
            {
                if (Type.GetType(bottomRewardPresentation.rewardType) == forReward.GetType())
                {
                    return bottomRewardPresentation;
                }
            }
            return null;
        }

        public void StartCollectingBottomRewards()
        {
            foreach (var rewardPresentation in bottomRewardPresentations)
                rewardPresentation.StartCollectingRewardEffect(hudPresentationController);
        }

        public void AddTask(LevelWinPopupTask task, int priority, string id)
        {
            taskChain.AddTask(task, priority, id);
        }

        public void Skip()
        {
            if (canSkip)
            {
                canSkip = false;
                blockerObject.SetActive(true);
                onSkip.Invoke();
            }
        }

        private void SetCanSkip()
        {
            blockerObject.SetActive(false);
            canSkip = true;
        }

        public void OnExitClick()
        {
            onExit();
        }

        public void OverrideExiting(Action onExit)
        {
            this.onExit = onExit;
        }
        
        public override void Back()
        {
        }
    }
}