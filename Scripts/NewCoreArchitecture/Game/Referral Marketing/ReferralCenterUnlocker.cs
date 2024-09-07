using System;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.Game.ReferralMarketing
{

    public class ReferralCenterUnlocker : EventListener
    {
        readonly Action unlockingAction;
        private int unlockingLevelIndex;
        
        public ReferralCenterUnlocker(Action unlockingAction)
        {
            this.unlockingAction = unlockingAction;

            ServiceLocator.Find<ConfigurationManager>().Configure(this);
            ServiceLocator.Find<EventManager>().Register(this);

            GameProfiler.OnLastUnlockedLevelChange += i => CheckForUnlocking();
        }
        
        public void SetUnlockingLevelIndex(int levelIndex)
        {
            unlockingLevelIndex = levelIndex;
        }
        
        
        private void CheckForUnlocking()
        {
            if (!ShouldUnlock()) return;
            
            unlockingAction.Invoke();
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }
        
        private bool ShouldUnlock()
        {
            return Base.gameManager != null && Base.gameManager.profiler.LastUnlockedLevel >= unlockingLevelIndex;
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is GameOpenEvent || evt is ReferralCenterInitializedEvent)
            {
                CheckForUnlocking();
            }
        }
    }
    
}

