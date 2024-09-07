using Match3.Game.Gameplay.SubSystems.WinSequence;
using SeganX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{
    public class Popup_WinCeleberation : GameState
    {
        Action onSkipAction;
        Action onGoalsDoneMessageFinished;

        public void Setup(Action onSkipAction, Action onGoalsDoneMessageFinished)
        {
            this.onSkipAction = onSkipAction;
            this.onGoalsDoneMessageFinished = onGoalsDoneMessageFinished;
        }

        public void OnGoalsDoneMessageFinished()
        {
            onGoalsDoneMessageFinished();
        }

        public void Skip()
        {
            onSkipAction();
        }

        public override void Back()
        {
            
        }
    }
}