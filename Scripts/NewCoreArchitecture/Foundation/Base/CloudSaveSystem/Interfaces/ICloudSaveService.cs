using System;
using Match3.Foundation.Base.ServiceLocating;

namespace Match3.CloudSave
{
    public interface ICloudSaveService : Service
    {
        void SetServiceActive(bool active);
    
        void Authenticate(Action<AuthenticationStatus> onAuthenticate);
    
        void AddDataHandlers(params ICloudDataHandler[] dataHandlers);

        void Save(Action<CloudSaveRequestStatus> onSave);

        void TryResolveAuthenticationAndLoad(string authorizationKey, Action<CloudSaveRequestStatus> onLoad);
        
        bool IsAllDependenciesResolved();
        
        string GetLocalSavedAuthorizationKey();
    }
}


