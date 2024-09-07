using System;
using Match3.CloudSave;
using Match3.Network;
using UnityEngine;


namespace Match3.MedrickCloudSave
{

    public class MedrickCloudSaveSignInRequestHandler : MedrickCloudSaveServerRequestHandler
    {

        public void SignIn(MedrickCloudSaveImplementationController medrickCloudSaveController, Action<MedrickCloudSaveServerPlayerData> onSuccess, Action onFailure)
        {
            var request = CreateRequestBuilderFor("signin/")
                .SetBody(GetSignInRequestBody(medrickCloudSaveController.GetMedrickId(), medrickCloudSaveController.GetSavedAuthorizationKey()))
                .SetType(HTTPRequestType.POST).Build();
            RequestFromServer(request, successResult =>
            {
                var signUpResult = JsonUtility.FromJson<MedrickCloudSaveServerPlayerData>(successResult);
                onSuccess(signUpResult);
            }, failureResult =>
            {
                onFailure();
            });
        }
        
        private string GetSignInRequestBody(string medrickId, string playerToken)
        {
            var jsonBody = new NiceJson.JsonObject();
            jsonBody.Add("password", playerToken);
            jsonBody.Add("medrick_id", medrickId);
            return jsonBody.ToJsonString();
        }
        
    }

}