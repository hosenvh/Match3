using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Overlay.Analytics
{
    public class GameplayProgressAnalyticsHandler : GameplayAnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            //if (evt is GoalTargetDecreasedEvent goalTargetDecreasedEvent)
            //    AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Mission_Step(
            //        GoalTypeOf(goalTargetDecreasedEvent.goalTracker),
            //        CurrentAmount(goalTargetDecreasedEvent.goalTracker),
            //        TotalAmount(goalTargetDecreasedEvent.goalTracker)));

            //else if (evt is GoalTargetReachedEvent goalTargetReachedEvent)
            //    AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Mission_Done(
            //        GoalTypeOf(goalTargetReachedEvent.goalTracker)));

        }

        private int TotalAmount(GoalTracker goalTracker)
        {
            return goalTracker.goalAmount;
        }

        private int CurrentAmount(GoalTracker goalTracker)
        {
            return goalTracker.CurrentAmount();
        }

        private string GoalTypeOf(GoalTracker goalTracker)
        {
            var goalType = goalTracker.goalType;
            if (goalType is ColoredGoalType coloredGoalType)
                return string.Format("{0}_{1}", coloredGoalType.type.Name, coloredGoalType.color);
            else if (goalType is SimpleGoalType simpleGoalType)
                return simpleGoalType.type.Name;

            return "Error";
        }
    }
}