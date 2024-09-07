using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;
using Match3.Game.Gameplay.Swapping;
using Match3.Presentation.Gameplay;

namespace Match3.Overlay.Analytics
{
    public class GameplayUserInteractionsAnalyticsHandler : GameplayAnalyticsHandler
    {
        // bool isInExtraMoves;

        protected override void Handle(GameEvent evt)
        {
            // if (evt is LevelStartedEvent)
            // {
            //     isInExtraMoves = false;
            // }
            // else if (evt is LevelResumeWithExtraMoveEvent)
            // {
            //     isInExtraMoves = true;
            // }

            //else if (evt is UserMovedEvent)
            //{
            //    if (isInExtraMoves)
            //        AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Extra_Move());
            //    else
            //        AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Normal_Success());
            //}
            //else if (evt is SwapFailedEvent)
            //    AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Move_Failed());

            //else if (evt is HammerPowerUpActivatedEvent)
            //    AnalyticsManager.SendEvent(new AnalyticsData_InGame_Powerup_Hammer());
            //else if (evt is BroomPowerUpActivatedEvent)
            //    AnalyticsManager.SendEvent(new AnalyticsData_InGame_Powerup_Sweeper());
            //else if (evt is HandPowerUpActivatedEvent)
            //    AnalyticsManager.SendEvent(new AnalyticsData_InGame_Powerup_Hand());
        }
    }
}