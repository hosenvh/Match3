using System;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using SeganX;
using UnityEngine;


namespace Match3.CloudSave
{
    public class GoogleCloudSaveImplementationController : ICloudSaveImplementationController
    {
        public GoogleCloudSaveImplementationController(bool debugMode)
        {
            var config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = debugMode;
            PlayGamesPlatform.Activate();
        }
        
        public void Authenticate(Action<bool> onAuthentication)
        {
            if(!IsAuthenticated())
                Social.localUser.Authenticate( auth =>
                {
                    onAuthentication(auth);
                });
            else
            {
                onAuthentication(IsAuthenticated());
            }
        }


        public bool IsAuthenticated()
        {
            return Social.Active.localUser.authenticated;
        }

        public bool IsDependenciesResolved()
        {
            return Utilities.IsApplicationInstalled("com.google.android.play.games");
        }

        public void Save(string fileName, string data, Action<CloudSaveRequestStatus> onSaved)
        {
            var dataByte = Encoding.ASCII.GetBytes(data);
            OpenFile(fileName, (status, meta) =>
            {
                if (status != SavedGameRequestStatus.Success)
                {
                    onSaved((CloudSaveRequestStatus) status);
                    return;
                }
                
                var update = new SavedGameMetadataUpdate.Builder().Build();
                ((PlayGamesPlatform) Social.Active).SavedGame.CommitUpdate(meta, update, dataByte,
                    (_status, _meta) => onSaved((CloudSaveRequestStatus) _status));
            });
        }

        public void LoadWithSavedAuthorizationKey(string fileName, Action<CloudSaveRequestStatus, string> onLoad)
        {
            LoadWithInputAuthorizationKey("", fileName, onLoad);
        }

        public void LoadWithInputAuthorizationKey(string authorizationKey, string fileName, Action<CloudSaveRequestStatus, string> onLoad)
        {
            OpenFile(fileName, (status, meta) =>
            {
                if (status != SavedGameRequestStatus.Success)
                {
                    onLoad((CloudSaveRequestStatus) status, null);
                    return;
                }

                ((PlayGamesPlatform) Social.Active).SavedGame.ReadBinaryData(meta, (status2, data) =>
                {
                    onLoad((CloudSaveRequestStatus) status2, Encoding.ASCII.GetString(data));
                });
            });
        }

        private void OpenFile(string fileName, Action<SavedGameRequestStatus, ISavedGameMetadata> onOpened)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadNetworkOnly,
                ConflictResolutionStrategy.UseOriginal, onOpened);
        }

        public string GetPlayerId()
        {
            return ((PlayGamesLocalUser) Social.localUser).id;
        }

        public string GetSavedAuthorizationKey()
        {
            return "";
        }

        public string GetImplementationName()
        {
            return "GoogleSavedGame";
        }
    }

}


