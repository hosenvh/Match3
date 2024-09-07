using System;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using UnityEngine;


namespace Match3.Game.ReferralMarketing.RequestHandling
{
    
    public abstract class ReferralCenterRequestHandler
    {
        private const string NO_INTERNET_CONNECTION = "NO_INTERNET_CONNECTION";
        
        protected readonly ServerConnection ServerConnection;
        protected string BaseUrl;
        
        private RequestSession requestSession;

        
        protected ReferralCenterRequestHandler()
        {
            ServerConnection = ServiceLocator.Find<ServerConnection>();
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public void SetServerUrl(string url)
        {
            BaseUrl = url;
        }

        protected void RequestFromServer(HTTPRequest request, Action<string> onSuccess, Action<string> onFailure)
        {
            requestSession = new RequestSession(request, onSuccess, onFailure);

            if (Application.internetReachability == NetworkReachability.NotReachable)
                onFailure(NO_INTERNET_CONNECTION);
            else
                ServerConnection.Request(request, onSuccess, onFailure);
        }

        public void Retry()
        {
            RequestFromServer(requestSession.request, requestSession.onSuccess, requestSession.onFailure);
        }
        
        protected bool IsNoInternetError(string msg)
        {
            return NO_INTERNET_CONNECTION.Equals(msg);
        }

        protected bool IsTimeOutError(string msg)
        {
            return ServerConnection.IsTimeOut(msg);
        }
        
        
    }

}