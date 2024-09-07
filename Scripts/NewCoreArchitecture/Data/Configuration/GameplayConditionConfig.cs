using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using UnityEngine;

namespace Match3.Data.Configuration
{
    public abstract class GameplayConditionConfig : ScriptableObject
    {
        public abstract GameplayCondition CreateCondition();
    }

}
