using System;



namespace Match3.Game.TaskManagement
{

    public class CheckPrivacyPolicyMapTask : MonoConditionalTask
    {
        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            var privacy = new PrivacyPolicyController();
            privacy.CheckPrivacyPolicy(onComplete);
        }

        protected override bool IsConditionSatisfied()
        {
            return true;
        }
    }

}