using System;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using Match3.Game.ReferralMarketing;

namespace Match3.Main
{
    public class ReferralCenterInitializationTask : BasicTask
    {
        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            var referralCenter = ServiceLocator.Find<ReferralCenter>();
            referralCenter.Initialize(() => {}, failureReason =>{});
            onComplete();
        }
    }
}
