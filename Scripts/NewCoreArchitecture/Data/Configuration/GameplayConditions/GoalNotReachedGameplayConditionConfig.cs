using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.GameplayConditions;
using UnityEngine;
using static BoardConfig;

namespace Match3.Data.Configuration.GameplayConditions
{
    [CreateAssetMenu(menuName = "Gameplay/Conditions/Goal Not Reached")]
    public class GoalNotReachedGameplayConditionConfig : GameplayConditionConfig
    {
        public GoalTypeInfo goalInfo;

        public override GameplayCondition CreateCondition()
        {
            return new GoalNotReachedGameplayCondition(goalInfo.ExtractGoalType());
        }
    }

}
