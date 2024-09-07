using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;
using UnityEngine;


namespace Match3.Foundation.Base.NotificationService
{
    
    [CreateAssetMenu(menuName = "NotificationSystem/NcResultNotification")]
    public class NcResultNotification : Notification
    {

        private const string ENTER_LOBBY_KEY = "NcNotification_EnterLobby";
        
        public override bool CheckEvent(GameEvent ev)
        {
            return ev is NeighborhoodChallengeLobbyOpenedEvent;
        }

        public override bool IsConditionsResolved(INotificationDataStorage dataStorage)
        {
            var enterLobbyCount = dataStorage.GetInt(ENTER_LOBBY_KEY, 0);
            enterLobbyCount++;
            dataStorage.SaveInt(ENTER_LOBBY_KEY, enterLobbyCount);
            return enterLobbyCount > 1;
        }

        public override TimeSpan GetScheduleTime()
        {
            return TimeSpan.FromSeconds((ServiceLocator.Find<NeighborhoodChallengeManager>().UserInfo().CurrentChallenge().endTime - DateTime.UtcNow).TotalSeconds);
        }
    }
    

}

