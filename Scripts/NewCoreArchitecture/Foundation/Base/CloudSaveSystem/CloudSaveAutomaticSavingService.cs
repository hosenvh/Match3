using System;
using Match3;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Utility.GolmoradLogging;
using SeganX;
using UnityEngine;


namespace Match3.CloudSave.Events
{
    public interface CloudLogTag : LogTag
    {
    }

    public class CloudSaveAutomaticSavingService : Service, EventListener
    {
        
        private readonly CloudSaveService cloudSaveService;
        
        
        public CloudSaveAutomaticSavingService(EventManager eventManager, CloudSaveService cloudSaveService)
        {
            eventManager.Register(this);
            this.cloudSaveService = cloudSaveService;
        }
        
        
        public void OnEvent(GameEvent evt, object sender)
        {
            if (!cloudSaveService.IsStatusAcceptableToSave()) return;
            if (!IsSafeToSave()) return;
            if (!ServiceLocator.Find<GameTransitionManager>().IsInMap()) return;
            
            try
            {
                if (evt is TaskDoneEvent || evt is PurchaseSuccessEvent)
                {
                    SaveOnCloud();
                }
                else if (evt is MapEnteredEvent mapEnteredEvent)
                {
                    if (!mapEnteredEvent.FirstEntering)
                    {
                        SaveOnCloud(); 
                    }
                }
            }
            catch (Exception e)
            {
                if(Base.gameManager!=null && Base.gameManager.CurrentState!=null)
                    Debug.LogError($"Error On Automatic Saving - Current State: {Base.gameManager.CurrentState} - Event type: {evt.GetType()} \n Stack Trace : {e.StackTrace}");
            }
            
            //LogEvents(evt);
        }


        private bool IsSafeToSave()
        {
            #if UNITY_EDITOR
                return false;
            #else
                return Base.gameManager.mapManager.IsInMap();
            #endif
        }
        

        private void SaveOnCloud()
        {
            cloudSaveService.Save(status =>
            {
                if (status == CloudSaveRequestStatus.Successful)
                    DebugPro.LogInfo<CloudLogTag>("AUTO SAVED!");
            }); 
        }

        private void LogEvents(GameEvent evt)
        {
            if (evt is TaskDoneEvent)
            {
                Debug.Log("******** Cloud Event Listener received TaskDoneEvent");
            }
            else if (evt is PurchaseSuccessEvent)
            {
                Debug.Log("******** Cloud Event Listener received PurchaseSuccessEvent");
            }
            else if (evt is MapEnteredEvent mapEnteredEvent)
            {
                Debug.Log($"******** Cloud Event Listener received mapEnteredEvent ({mapEnteredEvent.FirstEntering})");
            }
        }
        
        
    }



}


