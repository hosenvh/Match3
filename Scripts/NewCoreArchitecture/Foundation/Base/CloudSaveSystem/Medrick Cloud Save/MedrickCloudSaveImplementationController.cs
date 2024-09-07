using System;
using Match3.Data.Unity.PersistentTypes;
using Match3.MedrickCloudSave;
using UnityEngine;


namespace Match3.CloudSave
{

    public class MedrickCloudSaveImplementationController : ICloudSaveImplementationController
    {
        private const string MAIN_SAVE_FILE_NAME = "mainSave";
        private const string AUTHENTICATION_SAVE_FILE_NAME = "authenticationSave";
        
        
        private PersistentString playerToken;
        private PersistentString playerAccess;
        private PersistentString playerRefresh;
        private PersistentString medrickId;
        private PersistentInt playerId;
        
        
        public MedrickCloudSaveServerPlayerData PlayerServerData { get; private set; }
        public bool IsServerDataExpired = false;
        
        private enum FilePathType {AuthDataFile, SaveDataFile, None}
        

        public MedrickCloudSaveImplementationController(bool debugMode)
        {
            playerToken = new PersistentString("MedrickCloudSave_PlayerToken");
            playerAccess = new PersistentString("MedrickCloudSave_PlayerAccess");
            playerRefresh = new PersistentString("MedrickCloudSave_PlayerRefresh");
            medrickId = new PersistentString("MedrickCloudSave_MedrickId");
            playerId = new PersistentInt("MedrickCloudSave_PlayerID");
        }
        
        
        
        public void Authenticate(Action<bool> onAuthentication)
        {
            if (IsAuthenticated())
                onAuthentication(true);
            else
            {
                if (!IsPlayerSignedUp())
                {
                    SignUp(OnSuccessfulAuthenticate, () => onAuthentication(false));
                }
                else
                {
                    SignIn(OnSuccessfulAuthenticate, () => onAuthentication(false));
                }
            }
            
            void OnSuccessfulAuthenticate(MedrickCloudSaveServerPlayerData signData)
            {
                PlayerServerData = signData;
                IsServerDataExpired = false;
                onAuthentication(true);
            }
        }

        private bool IsPlayerSignedUp()
        {
            return !GetMedrickId().IsNullOrEmpty();
        }
        
        private void SignUp(Action<MedrickCloudSaveServerPlayerData> onSuccess, Action onFailure)
        {
            new MedrickCloudSaveSignUpRequestHandler().SignUp(serverPlayerData =>
            {
                SavePlayerIdsAndToken(serverPlayerData);
                onSuccess(serverPlayerData);
            }, onFailure);
        }

        private void SignIn(Action<MedrickCloudSaveServerPlayerData> onSuccess, Action onFailure)
        {
            new MedrickCloudSaveSignInRequestHandler().SignIn(this, onSuccess,
                onFailure);
        }
        
        
        
        public bool IsAuthenticated()
        {
            return PlayerServerData != null && !PlayerServerData.login_info.username.IsNullOrEmpty() && !IsServerDataExpired;
        }

        public bool IsDependenciesResolved()
        {
            return true;
        }

        
        
        public void Save(string fileName, string data, Action<CloudSaveRequestStatus> onSaved)
        {
            var filePathType = GetFilePathType(fileName);
            var dataInFile = PutDataInFileByFilePathType(filePathType, data);
            var saveRequestHandler = new MedrickCloudSaveSaveDataRequestHandler();
            saveRequestHandler.SaveData(this, dataInFile, onSaved);
        }

        public void LoadWithSavedAuthorizationKey(string fileName, Action<CloudSaveRequestStatus, string> onLoad)
        {
            LoadWithInputAuthorizationKey(GetSavedAuthorizationKey(), fileName, onLoad);
        }

        public void LoadWithInputAuthorizationKey(string authorizationKey, string fileName, Action<CloudSaveRequestStatus, string> onLoad)
        {
            var loadRequest = new MedrickCloudSaveLoadDataRequestHandler();
            loadRequest.LoadData(this, authorizationKey.ToUpper(), (status, serverPlayerData) =>
            {
                if (status == CloudSaveRequestStatus.Successful)
                {
                    SavePlayerIdsAndToken(serverPlayerData);
                    var extractedData = ExtractDataByFilePath(fileName, serverPlayerData) ?? "";
                    onLoad(status, extractedData);
                }
                else
                    onLoad(status, "");
            });
        }

        private FilePathType GetFilePathType(string path)
        {
            if (path == AUTHENTICATION_SAVE_FILE_NAME)
                return FilePathType.AuthDataFile;
            else if (path == MAIN_SAVE_FILE_NAME)
                return FilePathType.SaveDataFile;
            
            Debug.LogError("Cloud Save | Medrick Implementation | Try save on unknown file path");
            return FilePathType.None;
        }

        private string PutDataInFileByFilePathType(FilePathType filePathType, string data)
        {
            var jsonObject = new NiceJson.JsonObject();
            switch (filePathType)
            {
                case FilePathType.AuthDataFile:
                    jsonObject.Add("auth_data", data);
                    break;
                case FilePathType.SaveDataFile:
                    jsonObject.Add("save_data", data);
                    break;
            }

            return jsonObject.ToJsonString();
        }

        private string ExtractDataByFilePath(string filePath, MedrickCloudSaveServerPlayerData data)
        {
            var filePathType = GetFilePathType(filePath);
            switch (filePathType)
            {
                case FilePathType.SaveDataFile:
                    return data.save_data;
                case FilePathType.AuthDataFile:
                    return data.auth_data;
                default:
                    return "";
            }
        }
        
        
        public string GetPlayerId()
        {
            return playerId.Get().ToString();
        }

        public string GetSavedAuthorizationKey()
        {
            return playerToken.Get();
        }

        public string GetSavedAuthorizationAccessKey()
        {
            return playerAccess.Get();
        }

        public string GetImplementationName()
        {
            return "MedrickCloudSave";
        }

        public string GetMedrickId()
        {
            return medrickId.Get();
        }


        private void SavePlayerIdsAndToken(MedrickCloudSaveServerPlayerData serverPlayerData)
        {
            playerId.Set(serverPlayerData.id);
            medrickId.Set(serverPlayerData.login_info.username);
            playerToken.Set(serverPlayerData.login_info.password);
            playerAccess.Set(serverPlayerData.credentials.access);
            playerRefresh.Set(serverPlayerData.credentials.refresh);
        }
        
        
    }

}