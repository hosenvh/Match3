using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.TaskManagement;
using Match3.Presentation.Gameplay;
using Match3.Presentation.HUD;
using UnityEngine;


namespace Match3.Game.TaskManagement
{
    public abstract class LevelWinPopupTask : Task
    {
        public Popup_WinBase winPopup;

        protected LevelWinPopupTask(Popup_WinBase winPopup)
        {
            this.winPopup = winPopup;
        }
        
        public void Execute(Action onComplete, Action onAbort)
        {
            ExecuteLevelWinPopupTask(onComplete);
        }

        protected abstract void ExecuteLevelWinPopupTask(Action onComplete);

        public float Progress()
        {
            return 1;
        }

        public abstract bool IsCompleted();
    }
}


