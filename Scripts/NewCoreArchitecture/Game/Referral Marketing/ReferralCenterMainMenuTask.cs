using System;
using Match3.Game.TaskManagement;

namespace NewCoreArchitecture.Game.Referral_Marketing
{
    public class ReferralCenterMainMenuTask : MainMenuTask
    {
        private readonly Action onExecute;
        private static bool isExecuted;

        public ReferralCenterMainMenuTask(Action onExecute)
        {
            this.onExecute = onExecute;
        }
        
        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            isExecuted = true;
            onExecute.Invoke();
            onComplete.Invoke();
        }

        protected override bool IsConditionSatisfied()
        {
            return !isExecuted;
        }
    }
}