using System;


namespace Match3.CloudSave
{
    public interface ICloudSaveImplementationController
    {
        void Authenticate(Action<bool> onAuthentication);
        bool IsAuthenticated();
        bool IsDependenciesResolved();
        void Save(string fileName, string data, Action<CloudSaveRequestStatus> onSaved);
        void LoadWithSavedAuthorizationKey(string fileName, Action<CloudSaveRequestStatus, string> onLoad);
        void LoadWithInputAuthorizationKey(string authorizationKey, string fileName, Action<CloudSaveRequestStatus, string> onLoad);
        string GetPlayerId();
        string GetSavedAuthorizationKey();
        string GetImplementationName();
    }
}


