using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using SeganX;
using System;
using Match3.Foundation.Base.EventManagement;


namespace Match3.Game.NeighborhoodChallenge
{
    public class NCLevelChangedEvent : GameEvent
    {
        public readonly int previousLevel;
        public readonly int newLevel;

        public NCLevelChangedEvent(int previousLevel, int newLevel)
        {
            this.previousLevel = previousLevel;
            this.newLevel = newLevel;
        }
    }

    public class NCLevelChangingController : NeighborhoodChallengeController
    {
        private int changeCost;
        private Action changeAction;

        // TODO: Remove the null.
        public NCLevelChangingController(NeighborhoodChallengeManager manager, Action changeAction) : base(manager, null)
        {
            this.changeAction = changeAction;

            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public void SetChangeCost(int cost)
        {
            this.changeCost = cost;
        }

        public int ChangeCost()
        {
            return changeCost;
        }

        public void ChangeLevel(Action onSuccess, Action onNotEnoughCoin)
        {
            if (Base.gameManager.profiler.CoinCount >= changeCost)
            {
                int previousLevel = manager.LevelSelector.SelectedLevel();
                Base.gameManager.profiler.ChangeCoin(-changeCost, "change-level");
                changeAction.Invoke();
                onSuccess.Invoke();
                ServiceLocator.Find<EventManager>().Propagate(new NCLevelChangedEvent(previousLevel: previousLevel, newLevel: manager.LevelSelector.SelectedLevel()), this);
            }
            else
                onNotEnoughCoin.Invoke();
        }
    }
}