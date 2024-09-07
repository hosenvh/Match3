using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.GameplayConditions;
using UnityEngine;

namespace Match3.Data.Configuration.GameplayConditions
{
    [CreateAssetMenu(menuName = "Gameplay/Conditions/Always True")]
    public class AlwaysTrueGameplayConditionConfig : GameplayConditionConfig
    {
        public override GameplayCondition CreateCondition()
        {
            return new AlwaysTrueGamepleplayCondition();
        }
    }

}
