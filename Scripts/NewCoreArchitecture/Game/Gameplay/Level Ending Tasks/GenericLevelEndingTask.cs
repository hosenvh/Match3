using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Presentation.Gameplay
{
    public class GenericLevelEndingTask : LevelEndingTask
    {
        private Action<Action> taskAction;

        public GenericLevelEndingTask(Action<Action> action)
        {
            this.taskAction = action;
        }

        protected override void ExecuteLevelEndingTask(Action onComplete)
        {
            taskAction(onComplete);
        }
    }    
}


