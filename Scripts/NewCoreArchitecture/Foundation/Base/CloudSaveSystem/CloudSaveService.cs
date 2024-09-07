using System;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using SeganX;
using UnityEngine;


namespace Match3.CloudSave
{
    
    public class CloudSaveService : ICloudSaveService
    {
        private const int ACCEPTABLE_SERVER_AUTHENTICATION_FAILED_COUNT = 3;
        
        
        // ------------------------------------------- Properties ------------------------------------------- \\ 

        public AuthenticationStatus Status { get; private set; } = AuthenticationStatus.NotAuthenticated;

        public CloudSaveRequestStatus LastSaveRequestStatus { get; private set; } = CloudSaveRequestStatus.NotRequested;
        public CloudSaveRequestStatus LastLoadRequestStatus { get; private set; } = CloudSaveRequestStatus.NotRequested;

        public bool IsServerAuthenticated => cloudSaveManager.IsServerAuthenticated();

        public bool IsServiceActive { private set; get; }

        public bool IsAuthenticationInProgress { private set; get; } = false;

        public int ServerAuthenticationFailedIterations
        {
            get => PlayerPrefs.GetInt("ServerAuthenticationFailedIterations", 0);
            private set => PlayerPrefs.SetInt("ServerAuthenticationFailedIterations", value);
        }

        public bool HasSwitchedCloudServer
        {
            get;
            private set;
        }

        // ------------------------------------------- Public Fields ------------------------------------------- \\

        public event Action<AuthenticationStatus> OnAuthenticateCallBack;

        // ------------------------------------------- Private Fields ------------------------------------------- \\

        private string cloudSaveManagerType;
        private string cloudSaveImplementationControllerType;

        private bool debugMode;
        private ICloudDataStorage dataStorage;
        private ICloudSaveManager cloudSaveManager;
        public ICloudSaveImplementationController cloudSaveImplementationController { get; private set; }

        // ==================================================================================================== \\



        public CloudSaveService(bool debugMode)
        {
            this.debugMode = debugMode;
            dataStorage = new JsonCloudDataStorage();
        }

        public void Setup(string cloudSaveManagerType, string cloudSaveImplementationControllerType)
        {
            SetCloudTypeVariables();
            var lastUsedCloudServer = PlayerPrefs.GetString("LastUsedCloudServer");
            if (lastUsedCloudServer.IsNullOrEmpty() == false && lastUsedCloudServer != this.cloudSaveImplementationControllerType)
                HasSwitchedCloudServer = true;
            PlayerPrefs.SetString("LastUsedCloudServer", this.cloudSaveImplementationControllerType);
            CreateCloudSaveManager(debugMode);

            void SetCloudTypeVariables()
            {
                if (IsCloudTypeValid())
                {
                    this.cloudSaveImplementationControllerType = cloudSaveImplementationControllerType;
                    this.cloudSaveManagerType = cloudSaveManagerType;
                }
                else
                {
                    this.cloudSaveImplementationControllerType = typeof(GoogleCloudSaveImplementationController).ToString();
                    this.cloudSaveManagerType = typeof(GameCloudSaveManager).ToString();
                }
            }

            bool IsCloudTypeValid()
            {
                return !cloudSaveManagerType.IsNullOrEmpty() && !cloudSaveImplementationControllerType.IsNullOrEmpty();
            }
        }

        private void CreateCloudSaveManager(bool debugMode)
        {
            cloudSaveImplementationController =
                Activator.CreateInstance(Type.GetType(cloudSaveImplementationControllerType), new object[]{debugMode}) as
                    ICloudSaveImplementationController;
            cloudSaveManager = Activator.CreateInstance(Type.GetType(cloudSaveManagerType),
                cloudSaveImplementationController, dataStorage, debugMode) as ICloudSaveManager;
        }
        
        
        public void SetServiceActive(bool active)
        {
            IsServiceActive = active;
        }
        

        public void Authenticate(Action<AuthenticationStatus> onAuthenticate)
        {
            if (!IsServiceActive)
            {
                Status = AuthenticationStatus.ServiceDeactivated;
                onAuthenticate(Status);
                return;
            }

            if (!Utilities.IsConnectedToInternet())
            {
                Status = AuthenticationStatus.AuthenticationFailed;
                onAuthenticate(Status);
                return;
            }

            IsAuthenticationInProgress = true;
            
            cloudSaveManager.Authenticate((isCloudSaveManagerAuthenticated, authenticateData) =>
            {
                if (isCloudSaveManagerAuthenticated)
                {
                    Status = CheckAuthenticationConflict(authenticateData);
                    ServerAuthenticationFailedIterations = 0;
                }
                else
                {
                    Status = AuthenticationStatus.AuthenticationFailed;
                    if(!IsServerAuthenticated)
                        ServerAuthenticationFailedIterations++;
                }

                OnAuthenticateCallBack?.Invoke(Status);
                onAuthenticate?.Invoke(Status);
                
                IsAuthenticationInProgress = false;
            });
        }


        public bool IsTryingToAuthenticateAcceptable()
        {
            return ServerAuthenticationFailedIterations < ACCEPTABLE_SERVER_AUTHENTICATION_FAILED_COUNT;
        }

        public bool IsStatusAcceptableToSave()
        {
            return Status == AuthenticationStatus.Successful ||
                   Status == AuthenticationStatus.SameEmptyAccount;
        }
        
        
        public void AddDataHandlers(params ICloudDataHandler[] dataHandlers)
        {
            foreach (var dataHandler in dataHandlers)
            {
                cloudSaveManager.AddDataHandler(dataHandler);
            }
        }
        

        public void Save(Action<CloudSaveRequestStatus> onSave)
        {
            Authenticate(status =>
            {
                if(IsStatusAcceptableToSave())
                    cloudSaveManager.Save(requestStatus =>
                    {
                        LastSaveRequestStatus = requestStatus;
                        onSave(requestStatus);
                    });
                else
                {
                    LastSaveRequestStatus = CloudSaveRequestStatus.AuthenticationError;
                    onSave(LastSaveRequestStatus);                    
                }
            });
        }


        public void TryResolveAuthenticationAndLoad(string authorizationKey, Action<CloudSaveRequestStatus> onLoad)
        {
            Authenticate(status =>
            {
                Status = status;

                if (Status == AuthenticationStatus.AuthenticationFailed || Status == AuthenticationStatus.NotAuthenticated ||
                    Status == AuthenticationStatus.DifferentEmptyAccount || Status == AuthenticationStatus.SameEmptyAccount 
                    || Status == AuthenticationStatus.ServiceDeactivated || Status == AuthenticationStatus.olderAppVersion)
                {
                    LastLoadRequestStatus = CloudSaveRequestStatus.AuthenticationError;
                    onLoad(LastLoadRequestStatus);
                    return;
                }

                if (Status == AuthenticationStatus.DifferentDeviceWithEmptyLocal || Status == AuthenticationStatus.DifferentDeviceWithFullLocal || 
                    Status == AuthenticationStatus.DifferentFullAccount || Status == AuthenticationStatus.DataNotSync)
                {
                    cloudSaveManager.Load(authorizationKey, loadRequestStatus =>
                    {
                        if (loadRequestStatus == CloudSaveRequestStatus.Successful)
                        {
                            cloudSaveManager.SaveAuthenticationData(authSaveRequestStatus =>
                            {
                                if (authSaveRequestStatus == CloudSaveRequestStatus.Successful)
                                {
                                    Status = AuthenticationStatus.Successful;
                                    LastLoadRequestStatus = CloudSaveRequestStatus.Successful;
                                }
                                else
                                {
                                    LastLoadRequestStatus = authSaveRequestStatus;
                                }
                                
                                onLoad(LastLoadRequestStatus);
                            });
                        }
                        else
                        {
                            LastLoadRequestStatus = loadRequestStatus;
                            onLoad(LastLoadRequestStatus);
                        }
                    });

                    return;
                }

                if (Status == AuthenticationStatus.Successful)
                {
                    cloudSaveManager.Load(authorizationKey, requestStatus =>
                    {
                        bool isGooglePlaySave = Type.GetType(ServiceLocator.Find<ServerConfigManager>().data.config.cloudSaveServiceConfig.cloudSaveImplementationControllerType) == typeof(GoogleCloudSaveImplementationController);
                        if (isGooglePlaySave)
                        {
                            LastLoadRequestStatus = requestStatus;
                            onLoad(requestStatus);
                            return;
                        }

                        if (requestStatus == CloudSaveRequestStatus.Successful)
                        {
                            cloudSaveManager.SaveAuthenticationData(authSaveRequestStatus =>
                            {
                                if (authSaveRequestStatus == CloudSaveRequestStatus.Successful)
                                {
                                    Status = AuthenticationStatus.Successful;
                                    LastLoadRequestStatus = CloudSaveRequestStatus.Successful;
                                }
                                else
                                {
                                    LastLoadRequestStatus = authSaveRequestStatus;
                                }

                                onLoad(LastLoadRequestStatus);
                            });
                        }
                        else
                        {
                            LastLoadRequestStatus = requestStatus;
                            onLoad(LastLoadRequestStatus);
                        }


                    });
                }
                
            });
            
        }

        public void FakeLoad(string data)
        {
            cloudSaveManager.FakeLoad(data);
        }
        
        private AuthenticationStatus CheckAuthenticationConflict(CloudSaveAuthData authenticationData)
        {
            var status = AuthenticationStatus.NotAuthenticated;

            var cloudLoadedPlayerAuthId = authenticationData.playerAuthId;
            var cloudLoadedDeviceId = authenticationData.deviceId;
            var currentPlayerId = cloudSaveManager.GetPlayerId();

            var cloudIsEmpty = string.IsNullOrEmpty(cloudLoadedPlayerAuthId);
            var sameAccount = currentPlayerId == cloudSaveManager.GetLocalSavedPlayerId();
            var sameDevice = cloudLoadedDeviceId == SystemInfo.deviceUniqueIdentifier;

            var currentAppVersion = float.Parse(Application.version);
            var savedAppVersion = float.Parse(authenticationData.gameVersion);

            if (savedAppVersion > currentAppVersion)
            {
                status = AuthenticationStatus.olderAppVersion;
                return status;
            }
            
            if (cloudSaveManager.IsEverDataSentToServer())
            {
                if (cloudIsEmpty && sameAccount)
                    status = AuthenticationStatus.SameEmptyAccount;
                
                if(!cloudIsEmpty && sameAccount && sameDevice)
                    status = AuthenticationStatus.Successful;
                
                if (cloudIsEmpty && !sameAccount)
                    status = AuthenticationStatus.DifferentEmptyAccount;
                
                if(!cloudIsEmpty && !sameAccount)
                    status = AuthenticationStatus.DifferentFullAccount;
                
                if(!cloudIsEmpty && sameAccount && !sameDevice)
                    status = AuthenticationStatus.DifferentDeviceWithFullLocal;
            }
            else
            {
                if (cloudIsEmpty)
                    status = AuthenticationStatus.Successful;

                if (!cloudIsEmpty && sameDevice)
                    status = AuthenticationStatus.DataNotSync;
                
                if (!cloudIsEmpty && !sameDevice)
                    status = AuthenticationStatus.DifferentDeviceWithEmptyLocal;
            }

            return status;
        }


        public Type GetCloudSaveImplementationType()
        {
            return Type.GetType(cloudSaveImplementationControllerType);
        }
        
        public bool IsAllDependenciesResolved()
        {
            return cloudSaveManager.IsAllDependenciesResolved();
        }

        public string GetLocalSavedAuthorizationKey()
        {
            return cloudSaveImplementationController.GetSavedAuthorizationKey();
        }
    }


}