using System;
using System.Collections.Generic;
using Match3.Utility;
using UnityEngine;


namespace Match3.CloudSave
{
    public class GameCloudSaveManager : ICloudSaveManager
    {
        private const string DATA_SEND_PREFIX_KEY = "CloudSaveDataSent";
        private const string MAIN_SAVE_FILE_NAME = "mainSave";
        private const string AUTHENTICATION_SAVE_FILE_NAME = "authenticationSave";
        
        private const string SaveIterationKey = "_SaveIteration#";
        private const string CloudDataVersionKey = "_DataSentToCloud#";
        private const string Emailkey = "playerEmail";
        private const string GameVersionKey = "gameVersion";
        
        public const string PlayerAuthIdKey = "playerAuthId";
        public const string DeviceIdKey = "deviceId";

        private const int CloudDataVersion = 1;
        
        // ----------------------------------------------- Properties ----------------------------------------------- \\

        public bool IsManagerAuthenticated { get; private set; }

        public int SaveIteration
        {
            get => PlayerPrefs.GetInt(SaveIterationKey, 0);
            private set => PlayerPrefs.SetInt(SaveIterationKey, value);
        }

        private string LocalSavedPlayerAuthId
        {
            get => PlayerPrefs.GetString(PlayerAuthIdKey, "");
            set => PlayerPrefs.SetString(PlayerAuthIdKey, value);
        }

        private bool BackwardDataConverted_V1
        {
            get => PlayerPrefsEx.GetBoolean("CloudSaveBackwardDataConvert_1", false);
            set => PlayerPrefsEx.SetBoolean("CloudSaveBackwardDataConvert_1", value);
        }
        
        // --------------------------------------------- Private Fields --------------------------------------------- \\

        private List<ICloudDataHandler> dataHandlers = new List<ICloudDataHandler>();
        private ICloudSaveImplementationController cloudSaveImplementationController;
        private ICloudDataStorage dataStorage;
        
        // ========================================================================================================== \\
        
        public GameCloudSaveManager(ICloudSaveImplementationController cloudSaveImplementationController, ICloudDataStorage dataStorage, bool debugMode)
        {
            this.dataStorage = dataStorage;
            this.cloudSaveImplementationController = cloudSaveImplementationController;
            
            if (!BackwardDataConverted_V1)
            {
                CheckToConvertBackwardData_V1();
                BackwardDataConverted_V1 = true;
            }
        }

        public void Authenticate(Action<bool, CloudSaveAuthData> onAuthentication)
        {
            cloudSaveImplementationController.Authenticate((isAuthenticated) =>
            {
                if (isAuthenticated)
                {
                    LoadAuthenticationData( (status, data) =>
                    {
                        if (status == CloudSaveRequestStatus.Successful)
                        {
                            IsManagerAuthenticated = true;
                            onAuthentication(true, data);
                        }
                        else
                        {
                            IsManagerAuthenticated = false;
                            onAuthentication(false, null);
                        }
                    });
                }
                else
                {
                    IsManagerAuthenticated = false;
                    onAuthentication(false, null);
                }
            });
        }

        public void AddDataHandler(ICloudDataHandler cloudDataHandler)
        {
            dataHandlers.Add(cloudDataHandler);
        }

        public void Save(Action<CloudSaveRequestStatus> onSaved)
        {
            if (!IsManagerAuthenticated)
            {
                onSaved(CloudSaveRequestStatus.AuthenticationError);
                return;
            }
            
            dataStorage.Clear();
            CollectData(dataStorage);
            cloudSaveImplementationController.Save(MAIN_SAVE_FILE_NAME, dataStorage.SerializeData(), status =>
            {
                if (status == CloudSaveRequestStatus.Successful)
                {
                    SaveAuthenticationData(onSaved);
                }
                else
                {
                    var cloudSaveStatus = status;
                    onSaved(cloudSaveStatus);
                }
            });
        }

        public void Load(string authorizationKey, Action<CloudSaveRequestStatus> onLoaded)
        {
            if (!IsManagerAuthenticated)
            {
                onLoaded(CloudSaveRequestStatus.AuthenticationError);
                return;
            }

            cloudSaveImplementationController.LoadWithInputAuthorizationKey(authorizationKey, MAIN_SAVE_FILE_NAME, (status, data) =>
            {
                if (status == CloudSaveRequestStatus.Successful)
                {
                    if (NoSaveDataExists())
                    {
                        onLoaded(CloudSaveRequestStatus.NoSavedDataExists);
                        return;
                    }
                    dataStorage.Clear();
                    dataStorage.DeserializeData(data);
                    
                    foreach (var dataHandler in dataHandlers)
                    {
                        dataHandler.SpreadData(dataStorage);
                    }
                }
                
                onLoaded(status);
                
                bool NoSaveDataExists() => data == "";
            });
        }

        public void FakeLoad(string data)
        {
            dataStorage.Clear();

            dataStorage.DeserializeData(data);
                    
            foreach (var dataHandler in dataHandlers)
            {
                dataHandler.SpreadData(dataStorage);
            }
        }

        public void SaveAuthenticationData(Action<CloudSaveRequestStatus> onSaved)
        {
            dataStorage.Clear();
            dataStorage.SetString(PlayerAuthIdKey, GetPlayerId());
            dataStorage.SetString(DeviceIdKey, SystemInfo.deviceUniqueIdentifier);
            dataStorage.SetInt(SaveIterationKey, SaveIteration + 1);
            dataStorage.SetInt(CloudDataVersionKey, CloudDataVersion);
            dataStorage.SetString(GameVersionKey, Application.version);

            cloudSaveImplementationController.Save(AUTHENTICATION_SAVE_FILE_NAME, dataStorage.SerializeData(), status =>
            {
                if (status == CloudSaveRequestStatus.Successful)
                {
                    SaveIteration++;
                    LocalSavedPlayerAuthId = GetPlayerId();
                    SetDataSentToServer();
                }

                CloudSaveRequestStatus cloudSaveStatus = status;
                onSaved(cloudSaveStatus);
            });
        }

        private void LoadAuthenticationData(Action<CloudSaveRequestStatus, CloudSaveAuthData> onLoad)
        {
            cloudSaveImplementationController.LoadWithSavedAuthorizationKey( AUTHENTICATION_SAVE_FILE_NAME, (status, data) =>
            {
                if (status == CloudSaveRequestStatus.Successful)
                { 
                    dataStorage.Clear();
                    dataStorage.DeserializeData(data);
                    var cloudAuthData = new CloudSaveAuthData()
                    {
                        email = dataStorage.GetString(Emailkey, ""),
                        playerAuthId = dataStorage.GetString(PlayerAuthIdKey, ""),
                        deviceId = dataStorage.GetString(DeviceIdKey, SystemInfo.deviceUniqueIdentifier),
                        saveIteration = dataStorage.GetInt(SaveIterationKey, -1),
                        cloudDataVersion =  dataStorage.GetInt(CloudDataVersionKey, 1),
                        gameVersion = dataStorage.GetString(GameVersionKey, Application.version)
                    };
                    
                    onLoad(CloudSaveRequestStatus.Successful, cloudAuthData);
                }
                else
                {
                    onLoad(CloudSaveRequestStatus.AuthenticationError, null);
                }
            });
        }

        
        public string GetPlayerId()
        {
            return cloudSaveImplementationController.GetPlayerId();
        }

        public bool IsServerAuthenticated()
        {
            return cloudSaveImplementationController.IsAuthenticated();
        }

        public bool IsAllDependenciesResolved()
        {
            return cloudSaveImplementationController.IsDependenciesResolved();
        }

        
        
        public bool IsEverDataSentToServer()
        {
            return PlayerPrefsEx.GetBoolean(
                $"{DATA_SEND_PREFIX_KEY}_{cloudSaveImplementationController.GetImplementationName()}", false);
        }

        public void SetDataSentToServer()
        {
            PlayerPrefsEx.SetBoolean($"{DATA_SEND_PREFIX_KEY}_{cloudSaveImplementationController.GetImplementationName()}",
                true);
        }
        
        public string GetLocalSavedPlayerId()
        {
            return LocalSavedPlayerAuthId;
        }

        private void CollectData(ICloudDataStorage storage)
        {
            for (var i = dataHandlers.Count - 1; i >= 0; --i)
            {
                dataHandlers[i].CollectData(storage);
            }
        }

        private void CheckToConvertBackwardData_V1()
        {
            if (!LocalSavedPlayerAuthId.IsNullOrEmpty() &&
                cloudSaveImplementationController.GetImplementationName() == "GoogleSavedGame")
                SetDataSentToServer();
            if (cloudSaveImplementationController.GetImplementationName() == "MedrickCloudSave")
                LocalSavedPlayerAuthId = "";
        }
        
    }    
}


