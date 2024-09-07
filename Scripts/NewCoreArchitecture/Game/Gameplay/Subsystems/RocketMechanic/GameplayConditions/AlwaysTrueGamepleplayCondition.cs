namespace Match3.Game.Gameplay.SubSystems.RocketMechanic.GameplayConditions
{
    public class AlwaysTrueGamepleplayCondition : GameplayCondition
    {
        public bool IsSatisfied(GameplayController gameplayController)
        {
            return true;
        }
    }

}