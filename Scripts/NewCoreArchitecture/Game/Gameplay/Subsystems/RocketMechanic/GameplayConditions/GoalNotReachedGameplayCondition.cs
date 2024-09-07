using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.SubSystems.RocketMechanic.GameplayConditions
{
    public class GoalNotReachedGameplayCondition : GameplayCondition
    {
        GoalTargetType goalTarget;

        public GoalNotReachedGameplayCondition(GoalTargetType goalTarget)
        {
            this.goalTarget = goalTarget;
        }

        public bool IsSatisfied(GameplayController gameplayController)
        {
            foreach (var goalTracker in gameplayController.GetSystem<LevelStoppingSystem>().AllGoals())
                if (goalTracker.goalType.Is(goalTarget) && goalTracker.IsGoalReached())
                    return false;

            return true;
        }
    }

}