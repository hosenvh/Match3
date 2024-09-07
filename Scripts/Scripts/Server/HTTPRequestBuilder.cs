using System.Collections.Generic;

namespace Match3.Network
{
    public class HTTPRequestBuilder
    {
        private HTTPRequestType requestType;
        private string url;
        private string body;
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        private Dictionary<string, string> parameters = new Dictionary<string, string>();
        private float? timeout = null;

        public HTTPRequestBuilder SetType(HTTPRequestType type)
        {
            this.requestType = type;
            return this;
        }

        public HTTPRequestBuilder SetURL(string url)
        {
            this.url = url;
            return this;
        }

        public HTTPRequestBuilder SetBody(string body)
        {
            this.body = body;
            return this;
        }

        public HTTPRequestBuilder AddHeader(string key, string value)
        {
            headers[key] = value;
            return this;
        }

        // TODO: Rename this to AddParameter
        public HTTPRequestBuilder AddParameters(string key, string value)
        {
            parameters[key] = value;
            return this;
        }

        public HTTPRequestBuilder SetTimeout(float timeout)
        {
            this.timeout = timeout;
            return this;
        }

        public HTTPRequest Build()
        {
            return new HTTPRequest(requestType, url, body, headers, parameters, timeout);
        }


    }
}
