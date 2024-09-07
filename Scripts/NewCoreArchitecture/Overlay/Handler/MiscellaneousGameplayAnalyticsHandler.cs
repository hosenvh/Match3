using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;

namespace Match3.Overlay.Analytics
{
    public class MiscellaneousGameplayAnalyticsHandler : GameplayAnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            if(evt is ShufflingBoardEvent)
                AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Blocked());
        }
    }
}