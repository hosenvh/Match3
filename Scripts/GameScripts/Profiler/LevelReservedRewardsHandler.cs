using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Presentation.Gameplay;


namespace Match3.Profiler
{
    public class LevelReservedRewardsHandler : EventListener
    {
        private readonly List<Type> reservedsRewardsTypes = new List<Type>();

        public LevelReservedRewardsHandler()
        {
            ServiceLocator.Find<EventManager>().Register(this);
        }

        public void OnEvent(GameEvent gameEvent, object sender)
        {
            if ((gameEvent is LevelGaveUpEvent
                 || gameEvent is LevelRetriedEvent
                 || (gameEvent is LevelEndedEvent data && data.result == LevelResult.Win)
                 || gameEvent is LevelAbortedEvent))
            {
                reservedsRewardsTypes.Clear();
            }
        }

        public List<Type> GetReservedRewardTypes()
        {
            return new List<Type>(reservedsRewardsTypes);
        }

        public bool DoesRewardOfTypeExist(Type rewardType)
        {
            return FindRewardOfType() != null;

            Type FindRewardOfType()
            {
                return reservedsRewardsTypes.Find(r => r == rewardType);
            }
        }

        public void AddReservedReward(Type rewardType)
        {
            reservedsRewardsTypes.Add(rewardType);
        }

        public void ConsumeReservedReward(Type rewardType)
        {
            reservedsRewardsTypes.Remove(rewardType);
        }
    }
}