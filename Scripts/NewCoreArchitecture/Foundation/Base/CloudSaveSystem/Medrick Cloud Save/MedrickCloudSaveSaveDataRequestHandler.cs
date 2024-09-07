using System;
using Match3.MedrickCloudSave;
using Match3.Network;


namespace Match3.CloudSave
{
    public class MedrickCloudSaveSaveDataRequestHandler : MedrickCloudSaveServerRequestHandler
    {
        public void SaveData(MedrickCloudSaveImplementationController medrickCloudSaveController, string jsonData, Action<CloudSaveRequestStatus> onSaved)
        {
            var request = CreateRequestBuilderFor($"{medrickCloudSaveController.GetPlayerId()}/update/")
                .AddHeader("Authorization", $"Bearer {medrickCloudSaveController.GetSavedAuthorizationAccessKey()}")
                .SetBody(jsonData).SetType(HTTPRequestType.POST).Build();
            RequestFromServer(request, successResult =>
            {
                onSaved(CloudSaveRequestStatus.Successful);
            }, failureResult =>
            {
                onSaved(CloudSaveRequestStatus.InternalError);
            });
        }
    }
}
