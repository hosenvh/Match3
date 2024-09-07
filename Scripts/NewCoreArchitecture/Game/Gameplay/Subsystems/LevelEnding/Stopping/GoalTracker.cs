

using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{

    public class GoalTracker
    {
        public event Action onGatheredAmountIncreasedEvent = delegate { };
        public event Action onGatheredAmountDecreasedEvent = delegate { };

        public readonly GoalTargetType goalType;
        public readonly int goalAmount;
        int gatheredAmount;

        EventManager eventManager;


        public GoalTracker(GoalTargetType goalType, int goalAmount)
        {
            this.goalType = goalType;
            this.goalAmount = goalAmount;
            this.gatheredAmount = 0;
            eventManager = ServiceLocator.Find<EventManager>();
        }

        public void TrackDestruction(GoalObject goal)
        {
            if (IsGoalReached())
                return;

            if (goalType.Includes(goal))
            {
                gatheredAmount++;
                onGatheredAmountIncreasedEvent.Invoke();
                eventManager.Propagate(new GoalTargetDecreasedEvent(this), this);
                if(IsGoalReached())
                    eventManager.Propagate(new GoalTargetReachedEvent(this), this);
            }
        }

        public void TrackGeneration(GoalObject goal)
        {
            if (DoesGenerationAffectGoalAmmount(goal) == false)
                return;

            if (goalType.Includes(goal))
            {
                gatheredAmount--;
                onGatheredAmountDecreasedEvent.Invoke();
                eventManager.Propagate(new GoalTargetIncreasedEvent(this), this);
            }
        }

        // TODO: Move this to a config
        bool DoesGenerationAffectGoalAmmount(GoalObject goal)
        {
            return goal is Honey ||
                goal is IvyRootCell;
        }

        public int RemainingAmount()
        {
            return goalAmount - gatheredAmount;
        }

        public bool IsGoalReached()
        {
            return gatheredAmount >= goalAmount;
        }

        public int CurrentAmount()
        {
            return gatheredAmount;
        }


        // NOTE: This is only for testing purposes
        public void SetRemainingAmount(int amount)
        {
            gatheredAmount = goalAmount - amount;
        }
    }
}