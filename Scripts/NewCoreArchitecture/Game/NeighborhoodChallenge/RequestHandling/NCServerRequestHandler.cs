using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;
using System;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{


    public abstract class NCServerRequestHandler
    {
        const string NO_INTERNET_CONNECTION = "NO_INTERNET_CONNECTION";

        ServerConnection serverConnection;
        string baseUrl;
        RequestSession requestSession;

        public void SetServerURL(string serverURL)
        {
            this.baseUrl = serverURL;
        }

        public NCServerRequestHandler()
        {
            this.serverConnection = ServiceLocator.Find<ServerConnection>();
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        protected NCHTTPRequestBuilder CreateRequestBuilderFor(string relativeUrl)
        {
            var builder = new NCHTTPRequestBuilder();
            builder.SetURL($"{baseUrl}/{relativeUrl}");
            builder.AddHeader("Content-type", "application/json");
            return builder;
        }

        protected void RequestFromServer(HTTPRequest request, Action<string> onSuccess, Action<string> onFailure)
        {
            // TODO: Find a solution for clearing the session.
            requestSession = new RequestSession(request, onSuccess, onFailure);

            if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
                onFailure(NO_INTERNET_CONNECTION);
            else
                serverConnection.Request(request, onSuccess, onFailure);
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
            return serverConnection.IsTimeOut(msg);
        }
    }
}