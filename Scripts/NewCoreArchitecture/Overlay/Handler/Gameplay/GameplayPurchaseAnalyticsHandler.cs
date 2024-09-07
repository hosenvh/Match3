using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Presentation.Gameplay.PowerUpActivation;

namespace Match3.Overlay.Analytics
{
    public class GameplayPurchaseAnalyticsHandler : GameplayAnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            var powerUpName = "Error";
            if (evt is PowerUpPurchaseEvent powerUpPurchaseEvent)
                powerUpName = PowerOfNameOf(powerUpPurchaseEvent.powerupIndex);
            
            if(evt is PowerUpPurchaseSucceededEvent powerUpPurchaseSucceededEvent)
                AnalyticsManager.SendEvent(new AnalyticsData_InGame_PowerUp_Success(CostOf(powerUpPurchaseSucceededEvent.powerupIndex), powerUpName));

            else if(evt is PowerUpPurchaseFailedEvent)
                AnalyticsManager.SendEvent(new AnalyticsData_InGame_PowerUp_Failed(powerUpName));

            else if (evt is PowerUpPurchaseOpenedEvent)
                AnalyticsManager.SendEvent(new AnalyticsData_InGame_PowerUp_Popup(powerUpName));

            else if (evt is PowerUpPurchaseClosedEvent)
                AnalyticsManager.SendEvent(new AnalyticsData_InGame_PowerUp_Close(powerUpName));
        }



        private int CostOf(int powerupIndex)
        {
            return Base.gameManager.profiler.GetPowerupPrice(powerupIndex);
        }
    }
}