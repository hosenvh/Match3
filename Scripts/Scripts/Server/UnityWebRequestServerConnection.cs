using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Match3.Utility.GolmoradLogging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
#pragma warning disable CS0618

namespace Match3.Network
{
    public class UnityWebRequestServerConnection : ServerConnection
    {
        // A temporary way to detecting timeout error by callers.
        // TODO: Find a better way.
        const string TIME_OUT_ERROR_MESSAGE = "Timeout";
        private const string AUTHORIZATION_ERROR_MESSAGE = " Authorization";

        float defaultTimeout = 5f;


        public void SetDefaultTimeOut(float timeOut)
        {
            this.defaultTimeout = timeOut;
        }

        public void Request(HTTPRequest request, Action<string> onSuccess, Action<string> onFailure)
        {
            var webRequest = CreateWebRequestFrom(request);
            webRequest.timeout = (int)(request.timeOut != null ? request.timeOut : defaultTimeout);
            onSuccess += s => { webRequest.Dispose(); };
            onFailure += s => { webRequest.Dispose(); };
            DebugPro.LogInfo<ServerLogTag>($"Request | Type: {request.requestType} | UsedTimeOutDuration: {webRequest.timeout} | URL: {webRequest.url}");
            ServiceLocator.Find<UnityTimeScheduler>().StartCoroutine(Connecting(webRequest, onSuccess, onFailure));
        }

        private UnityWebRequest CreateWebRequestFrom(HTTPRequest request)
        {
            UnityWebRequest webRequest;

            switch (request.requestType)
            {
                case HTTPRequestType.POST:
                    var postData = Encoding.UTF8.GetBytes(request.body);
                    // Note: It seems giving request.body directly to UnityWebRequest doesn't work.
                    webRequest = new UnityWebRequest(request.url, method:  UnityWebRequest.kHttpVerbPOST);
                    webRequest.uploadHandler = new UploadHandlerRaw(postData);
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    AddHeaders(webRequest, request.headers);

                    webRequest.disposeUploadHandlerOnDispose = true;
                    webRequest.disposeDownloadHandlerOnDispose = true;

                    return webRequest;

                case HTTPRequestType.GET:
                    var parameters = FormatedParameters(request.parameters);
                    webRequest = UnityWebRequest.Get(request.url + parameters);
                    AddHeaders(webRequest, request.headers);
                    return webRequest;
            }

            return null;
        }

        private void AddHeaders(UnityWebRequest unityWebRequest, Dictionary<string, string> headers)
        {
            foreach (var element in headers)
                unityWebRequest.SetRequestHeader(element.Key, element.Value);
        }

        private string FormatedParameters(Dictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return "";
            var resultBuilder = new StringBuilder("?");

            foreach (var pair in parameters)
            {
                resultBuilder.Append(pair.Key);
                resultBuilder.Append("=");
                resultBuilder.Append(pair.Value);
                resultBuilder.Append("&");
            }

            // NOTE: Removing the last '&'.
            // WARNING: This implementation may be dangerous.
            resultBuilder.Length--;

            return resultBuilder.ToString();
        }


        // TODO: Refactor this shit.
        // TODO: Try to Remove/Disable the logs
        private IEnumerator Connecting(UnityWebRequest request, Action<string> successCallback, Action<string> failCallback)
        {
            yield return request.SendWebRequest();

            var response = request.downloadHandler.text;

            if (request.isDone)
                DebugPro.LogInfo<ServerLogTag>($"SERVER | Response | URL: {request.url} | UsedTimeOutDuration: {request.timeout} | Message:'\n'{TryFormatServerResponse(response)}");

            if (request.isNetworkError || request.isHttpError)
            {
                // TODO: Find a better way to determine timeout.
                if ("Request timeout".Equals(request.error))
                {
                    DebugPro.LogWarning<ServerLogTag>(message:$"Warning: {TIME_OUT_ERROR_MESSAGE} | Url: {request.url} | TimeoutDuration: {request.timeout}");
                    failCallback(TIME_OUT_ERROR_MESSAGE);
                } else if ("HTTP/1.1 401 Unauthorized".Equals(request.error) || "HTTP/1.1 403 Forbidden".Equals(request.error))
                {
                    DebugPro.LogInfo<ServerLogTag>(message: $"{AUTHORIZATION_ERROR_MESSAGE}: {request.error} | ResponseCode: {request.responseCode} | Message: {response} | Url: {request.url} ");
                    failCallback(response);
                }
                else
                {
                    // NOTE: request.responseCode == 0 is not considered as an error for now, but it may need further investigation.
                    if (Application.internetReachability != NetworkReachability.NotReachable && request.responseCode != 0)
                        DebugPro.LogError<ServerLogTag>(message: $"Error: {request.error} | ResponseCode: {request.responseCode} | Message: {response} | Url: {request.url} ");
                    failCallback(response);
                }
            }
            else
                successCallback(response);
        }

        private string TryFormatServerResponse(string response)
        {
            try
            {
                return JToken.Parse(response).ToString(Formatting.Indented);
            }
            catch (Exception)
            {
                return response;
            }
        }

        public bool IsTimeOut(string msg)
        {
            return TIME_OUT_ERROR_MESSAGE.Equals(msg);
        }
    }
}