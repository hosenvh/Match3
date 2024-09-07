using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ReferralMarketing;
using UnityEngine;



namespace Match3.Game.TaskManagement
{

    public class MainMenuReferralCenterTutorialTask : MainMenuTask
    {
        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            Base.gameManager.tutorialManager.CheckThenShowTutorial(74, 0, null, onAbort);
        }

        protected override bool IsConditionSatisfied()
        {
            var referralCenter = ServiceLocator.Find<ReferralCenter>();
            return referralCenter.IsUnlocked && !Base.gameManager.tutorialManager.IsTutorialShowed(74);
        }
    }

}