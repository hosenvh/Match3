namespace Match3.Game.Gameplay.SubSystems.RocketMechanic
{
    public interface GameplayCondition
    {
        bool IsSatisfied(GameplayController gameplayController);
    }
}