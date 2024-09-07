using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Presentation.Gameplay;

namespace Match3.Overlay.Analytics
{
    public abstract class GameplayAnalyticsHandler : AnalyticsHandler
    {
        protected bool ShouldDecrementHeart()
        {
            if (ServiceLocator.Find<ILife>().IsInInfiniteLife())
                return false;

            if (Base.gameManager.CurrentState is GameplayState)
            {
                return Base.gameManager.CurrentState.As<GameplayState>().gameplayController.GetSystem<LevelStartResourceConsumingSystem>().IsLevelStartResourcesConsumed;
            }

            return true;
        }

        protected string PowerOfNameOf(int powerupIndex)
        {
            switch (powerupIndex)
            {
                case 0:
                    return AnalyticsDataMaker.HAMMER_POWERUP;
                case 1:
                    return AnalyticsDataMaker.BROOM_POWERUP;
                case 2:
                    return AnalyticsDataMaker.HAND_POWERUP;
            }

            return "Error";
        }
    }
}