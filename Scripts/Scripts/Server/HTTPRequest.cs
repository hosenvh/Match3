using System;
using System.Collections.Generic;

namespace Match3.Network
{
    public enum HTTPRequestType { GET, POST }

    
    struct RequestSession
    {
        public readonly HTTPRequest request;
        public readonly Action<string> onSuccess;
        public readonly Action<string> onFailure;

        public RequestSession(HTTPRequest request, Action<string> onSuccess, Action<string> onFailure)
        {
            this.request = request;
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;
        }
    }
    
    
    public struct HTTPRequest
    {
        public readonly HTTPRequestType requestType;
        public readonly string url;
        public readonly string body;
        public readonly Dictionary<string, string> headers;
        public readonly Dictionary<string, string> parameters;
        public readonly float? timeOut;

        public HTTPRequest(
            HTTPRequestType requestType, 
            string url,
            string body,
            Dictionary<string, string> headers, 
            Dictionary<string, string> parameters,
            float? timeOut)
        {
            this.requestType = requestType;
            this.url = url;
            this.body = body;
            this.headers = headers;
            this.parameters = parameters;
            this.timeOut = timeOut;
        }
    }
}
