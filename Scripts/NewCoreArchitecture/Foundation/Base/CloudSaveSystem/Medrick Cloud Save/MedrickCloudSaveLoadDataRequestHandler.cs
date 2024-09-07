using System;
using Match3.MedrickCloudSave;
using Match3.Network;
using UnityEngine;


namespace Match3.CloudSave
{

    public class MedrickCloudSaveLoadDataRequestHandler : MedrickCloudSaveServerRequestHandler
    {
        public void LoadData(MedrickCloudSaveImplementationController medrickCloudSaveController, string authorizationKey, Action<CloudSaveRequestStatus, MedrickCloudSaveServerPlayerData> onLoad)
        {
            var request = CreateRequestBuilderFor("restore/")
                .AddHeader("Authorization", $"Bearer {medrickCloudSaveController.PlayerServerData.credentials.access}")
                .SetType(HTTPRequestType.GET)
                .AddParameters("key", authorizationKey).Build();
            RequestFromServer(request, successResult =>
            {
                var serverPlayerData = JsonUtility.FromJson<MedrickCloudSaveServerPlayerData>(successResult);
                onLoad(CloudSaveRequestStatus.Successful, serverPlayerData);
            }, failureResult =>
            {
                onLoad(CloudSaveRequestStatus.InternalError, null);
            });
        }
    }

}