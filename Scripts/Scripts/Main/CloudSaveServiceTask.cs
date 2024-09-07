using System;
using Match3.CloudSave;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;

namespace Match3.Main
{
    public class CloudSaveServiceTask : BasicTask
    {

        private readonly bool isFirstTimePlay = false;

        public CloudSaveServiceTask(bool isFirstTimePlay)
        {
            this.isFirstTimePlay = isFirstTimePlay;
        }

        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            onComplete();
            
            var secondTryToAuthenticate = false;
            
            var isCloudSaveActive = ServiceLocator.Find<ServerConfigManager>().data.config.isCloudSaveActive;
            var cloudService = ServiceLocator.Find<CloudSaveService>();
            cloudService.SetServiceActive(isCloudSaveActive);

            if(!isFirstTimePlay && cloudService.IsAllDependenciesResolved() && cloudService.IsTryingToAuthenticateAcceptable())
            {
                cloudService.Authenticate(cloudSaveServiceAuthenticationStatus =>
                {
                    if (cloudSaveServiceAuthenticationStatus == AuthenticationStatus.AuthenticationFailed && cloudService.IsServerAuthenticated &&
                        !secondTryToAuthenticate)
                    {
                        secondTryToAuthenticate = true;
                        cloudService.Authenticate(status2 => { });
                    }
                });
            }
        }
    }
}