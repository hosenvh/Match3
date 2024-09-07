using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Foundation.Base.EventManagement;
using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Configuration;

namespace Match3.Game.PiggyBank
{
    public class PiggyBankUnlockedEvent : GameEvent { }

    public class PiggyBankUnlocker : EventListener
    {
        private int unlockLevel;
        private Action onUnlock;

        public PiggyBankUnlocker(Action onUnlock)
        {
            this.onUnlock = onUnlock;
            ServiceLocator.Find<EventManager>().Register(this);
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public void TryUnlock()
        {
            if (!CanUnlock()) return;

            ServiceLocator.Find<EventManager>().UnRegister(this);
            onUnlock.Invoke();
        }

        public void SetUnlockLevel(int levelIndex)
        {
            unlockLevel = levelIndex;
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is MainMenuOpenEvent)
                TryUnlock();
        }

        private bool CanUnlock()
        {
            return Base.gameManager != null && Base.gameManager.profiler.LastUnlockedLevel >= unlockLevel;
        }
    }
}