using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.MedrickCloudSave;
using Match3.Network;


namespace Match3.CloudSave
{
    public class MedrickCloudSaveSendEmailRequestHandler : MedrickCloudSaveServerRequestHandler
    {
        public void SendPlayerEmail(string email, Action onSuccess, Action onFailed)
        {
            var cloudService = ServiceLocator.Find<CloudSaveService>();
            var cloudSaveController = (MedrickCloudSaveImplementationController) cloudService.cloudSaveImplementationController;
            var request = CreateRequestBuilderFor($"restore/email/?{email}")
                .AddHeader("Authorization", $"Bearer {cloudSaveController.PlayerServerData.credentials.access}")
                .SetType(HTTPRequestType.GET).Build();
            RequestFromServer(request, successResult =>
            {
                onSuccess();
            }, failedResult =>
            {
                onFailed();
            });
        }

        private string GetBody(string playerEmail)
        {
            var jsonObject = new NiceJson.JsonObject();
            jsonObject.Add("email", playerEmail);
            return jsonObject.ToJsonString();
        }
    }    
}


