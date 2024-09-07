using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using System;
using System.Collections.Generic;
using Match3.Utility.GolmoradLogging;


namespace Match3.Network
{
    public class MockServerConnection : ServerConnection
    {
        public class RequestEntry
        {
            public bool isActive;
            public string url;
            public bool shouldFaild;
            public string successMessage;
            public string failureMessage;
            public bool shouldDelegateToRealServer;
        }

        List<RequestEntry> requestEntries = new List<RequestEntry>();

        ServerConnection realServerConnection;

        public MockServerConnection()
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public void SetRealServerConnect(ServerConnection serverConnection)
        {
            this.realServerConnection = serverConnection;
        }

        public void Request(HTTPRequest request, Action<string> onSuccess, Action<string> onFailure)
        {
            var entry = FindRequestEntryFor(request);

            if (entry == null)
            {
                //onFailure($"Entry for {request.url} is not defined.");
                DebugPro.LogWarning<ServerLogTag>($"Entry for {request.url} is not defined.");
                realServerConnection.Request(request, onSuccess, onFailure);
                return;
            }

            if (entry.shouldDelegateToRealServer)
                realServerConnection.Request(request, onSuccess, onFailure);
            else
            {
                if (entry.shouldFaild)
                    onFailure(entry.failureMessage);
                else
                    onSuccess(entry.successMessage);
            }
        }

        public void AddRequestEntry(RequestEntry entry)
        {
            requestEntries.Add(entry);
        }

        private RequestEntry FindRequestEntryFor(HTTPRequest request)
        {
            return requestEntries.Find(e => e.isActive && request.url.Contains(e.url));
        }

        public bool IsTimeOut(string msg)
        {
            return true;
        }
    }
}