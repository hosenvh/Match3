using Match3.Foundation.Base.EventManagement;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{
    public class GoalTargetEvent : GameEvent
    {
        public readonly GoalTracker goalTracker;

        public GoalTargetEvent(GoalTracker goalTracker)
        {
            this.goalTracker = goalTracker;
        }
    }

    public class GoalTargetDecreasedEvent : GoalTargetEvent
    {
        public GoalTargetDecreasedEvent(GoalTracker goalTracker) : base(goalTracker)
        {
        }
    }

    public class GoalTargetIncreasedEvent : GoalTargetEvent
    {
        public GoalTargetIncreasedEvent(GoalTracker goalTracker) : base(goalTracker)
        {
        }
    }

    public class GoalTargetReachedEvent : GoalTargetEvent
    {
        public GoalTargetReachedEvent(GoalTracker goalTracker) : base(goalTracker)
        {
        }
    }
}