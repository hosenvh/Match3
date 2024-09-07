using System;
using Match3.CloudSave;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using UnityEngine;


namespace Match3.MedrickCloudSave
{

    public abstract class MedrickCloudSaveServerRequestHandler
    {
        const string NO_INTERNET_CONNECTION = "NO_INTERNET_CONNECTION";
        private const string TOKEN_NOT_VALID = "token_not_valid";
        const string BASE_URL = "https://mw.golmorad.medrick.info/api/player";

        ServerConnection serverConnection;
        RequestSession requestSession;


        public MedrickCloudSaveServerRequestHandler()
        {
            serverConnection = ServiceLocator.Find<ServerConnection>();
        }

        protected HTTPRequestBuilder CreateRequestBuilderFor(string relativeUrl)
        {
            var requestBuilder = new HTTPRequestBuilder();
            requestBuilder.SetURL($"{BASE_URL}/{relativeUrl}");
            requestBuilder.AddHeader("Content-type", "application/json");
            return requestBuilder;
        }

        protected void RequestFromServer(HTTPRequest request, Action<string> onSuccess, Action<string> onFailure)
        {
            requestSession = new RequestSession(request, onSuccess, onFailure);

            if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
                onFailure(NO_INTERNET_CONNECTION);
            else
                serverConnection.Request(request, onSuccess, onFailure: (string s) =>
                {
                    if (IsAccessKeyExpired(s))
                        HandleAccessExpiration();
                    else
                        onFailure.Invoke(s);
                });

            bool IsAccessKeyExpired(string responseString)
            {
                try
                {
                    var response = JsonUtility.FromJson<Response>(responseString);
                    return response.code == TOKEN_NOT_VALID;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Retry()
        {
            RequestFromServer(requestSession.request, requestSession.onSuccess, requestSession.onFailure);
        }

        private void HandleAccessExpiration()
        {
            var cloudService = ServiceLocator.Find<CloudSaveService>();
            var cloudSaveController = (MedrickCloudSaveImplementationController) cloudService.cloudSaveImplementationController;
            RetryWithFreshAccessToken();
            
            void RetryWithFreshAccessToken()
            {
                cloudSaveController.IsServerDataExpired = true;

                var request = CreateRequestBuilderFor("token/refresh/")
                    .SetBody("{\"refresh\":\"" + cloudSaveController.PlayerServerData.credentials.refresh + "\"}")
                    .AddHeader("Content-Type", "application/json")
                    .SetType(HTTPRequestType.POST).Build();

                serverConnection.Request(request, onSuccess: (s) =>
                {
                    var response = JsonUtility.FromJson<MedrickCloudSaveServerPlayerData>(s);
                    cloudSaveController.PlayerServerData.credentials = response.credentials;
                    UpdateCredentials();
                    Retry();
                }, onFailure: (s) =>
                {
                    ReAuthenticate();
                });
            }

            void ReAuthenticate()
            {
                cloudService.Authenticate(status =>
                {
                    if (status == AuthenticationStatus.Successful)
                    {
                        UpdateCredentials();
                        Retry();
                    }
                    else
                        requestSession.onFailure.Invoke(NO_INTERNET_CONNECTION);
                });
            }
        }

        private void UpdateCredentials()
        {
            var cloudService = ServiceLocator.Find<CloudSaveService>();
            var cloudSaveController = (MedrickCloudSaveImplementationController) cloudService.cloudSaveImplementationController;

            var previousRequestHeader = requestSession.request.headers;
            previousRequestHeader["Authorization"] = $"Bearer {cloudSaveController.PlayerServerData.credentials.access}";

            HTTPRequest request = new HTTPRequest(requestSession.request.requestType,
                requestSession.request.url,
                requestSession.request.body,
                previousRequestHeader,
                requestSession.request.parameters,
                requestSession.request.timeOut);

            requestSession = new RequestSession(request, requestSession.onSuccess, requestSession.onFailure);
            cloudSaveController.IsServerDataExpired = false;
        }

        protected bool IsNoInternetError(string msg)
        {
            return NO_INTERNET_CONNECTION.Equals(msg);
        }

        protected bool IsTimeOutError(string msg)
        {
            return serverConnection.IsTimeOut(msg);
        }
    }

    public class Response
    {
        public string detail;
        public string code;
        public Message[] messages;

        public class Message
        {
            public string token_class;
            public string token_type;
            public string message;
        }
    }

}