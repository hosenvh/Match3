using System;


namespace Match3.CloudSave
{
    public interface ICloudSaveManager
    {
        void Authenticate(Action<bool, CloudSaveAuthData> onAuthentication);
        
        void AddDataHandler(ICloudDataHandler cloudDataHandler);

        void Save(Action<CloudSaveRequestStatus> onSaved);

        void Load(string authorizationKey, Action<CloudSaveRequestStatus> onLoaded);

        void FakeLoad(string data);

        void SaveAuthenticationData(Action<CloudSaveRequestStatus> onSaved);
        
        bool IsServerAuthenticated();
        
        bool IsAllDependenciesResolved();
        
        string GetLocalSavedPlayerId();
        
        bool IsEverDataSentToServer();
        
        string GetPlayerId();
    }
}


