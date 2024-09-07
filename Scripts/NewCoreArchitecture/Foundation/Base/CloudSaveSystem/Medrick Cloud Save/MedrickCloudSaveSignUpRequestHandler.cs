using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using UnityEngine;


namespace Match3.MedrickCloudSave
{
    public class MedrickCloudSaveSignUpRequestHandler : MedrickCloudSaveServerRequestHandler
    {
        public void SignUp(Action<MedrickCloudSaveServerPlayerData> onSuccess, Action onFailure)
        {
            var request = CreateRequestBuilderFor("signup/").SetBody("{\"device_id\":\"" + SystemInfo.deviceUniqueIdentifier + "\"}").SetType(HTTPRequestType.POST).Build();
            RequestFromServer(request, successResult =>
            {
                var signUpResult = JsonUtility.FromJson<MedrickCloudSaveServerPlayerData>(successResult);
                onSuccess(signUpResult);
            }, failureResult =>
            {
                onFailure();
            });
        }

    }
}


