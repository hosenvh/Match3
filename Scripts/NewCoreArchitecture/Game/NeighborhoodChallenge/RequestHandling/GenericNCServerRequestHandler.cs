using Match3.Network;
using NiceJson;
using System;
using System.Diagnostics;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{

    public abstract class GenericNCServerRequestHandler<SuccessResultT> : NCServerRequestHandler
        where SuccessResultT : NCSuccessResult
    {

        // TODO: Remove this later.
        //Stopwatch stopWatch = new Stopwatch();

        protected NCRequestBody CreateBodyObject()
        {
            return new NCRequestBody();
        }

        protected void RequestFromServer(HTTPRequest request, Action<SuccessResultT> onSuccess, Action<NCFailureResult> onFailure)
        {
            //stopWatch.Restart();
            RequestFromServer(
                request,
                (msg) => HandleSuccessResponse(msg, onSuccess, onFailure),
                (err) => HandleFailureResponse(err, onFailure));
        }

        // TODO: Refactor this.
        private void HandleSuccessResponse(string msg, Action<SuccessResultT> onSuccess, Action<NCFailureResult> onFailure)
        {
            //stopWatch.Stop();
            //UnityEngine.Debug.Log($"Total Time for request {this.GetType().Name} was {stopWatch.ElapsedMilliseconds} ");

            object result = null;
            try
            {
                result = ExtractSuccessResult(NiceJson.JsonNode.ParseJsonString(msg)["data"]);

                if (result == null)
                    result = ExtractFailureResult(msg);
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

            if (result == null)
                result = CreateDefaultFailureResult();

            if (result is SuccessResultT successResult)
                onSuccess(successResult);
            else
                onFailure((NCFailureResult)result);
        }

        private void HandleFailureResponse(string msg, Action<NCFailureResult> onFailure)
        {

            //stopWatch.Stop();
            //UnityEngine.Debug.Log($"Total Time for request {this.GetType().Name} was {stopWatch.ElapsedMilliseconds} ");

            NCFailureResult result = null;

            try
            {
                if (IsTimeOutError(msg))
                    result = CreateTimeOutFailureResult();
                else if (IsNoInternetError(msg))
                    result = CreateNotInternetConnectionFailureResult();
                else
                    result = ExtractFailureResult(msg);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

            if (result == null)
                result = CreateDefaultFailureResult();

            onFailure(result);
        }


        protected abstract SuccessResultT ExtractSuccessResult(NiceJson.JsonNode body);

        protected virtual  NCFailureResult ExtractFailureResult(string error)
        {
            return CreateInternalServerError(error);
        }

        protected bool HasServerErrorCode(string error, string code)
        {
            try
            {
                var errorJson = JsonNode.ParseJsonString(error);
                return errorJson != null && errorJson.ContainsKey("msg") && errorJson["msg"].ToString().Equals(code);
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogError(e);
                return false;
            }
        }

        protected NCFailureResult CreateDefaultFailureResult()
        {
            return CreateInternalServerError("");
        }

        protected TimeOutFailureResult CreateTimeOutFailureResult()
        {
            return new TimeOutFailureResult() { retryAction = this.Retry };
        }

        protected InternalServerErrorFailureResult CreateInternalServerError(string errorMsg)
        {
            return new InternalServerErrorFailureResult() { message = errorMsg };
        }

        protected  NoInternetConnectionFailureResult CreateNotInternetConnectionFailureResult()
        {
            return new NoInternetConnectionFailureResult() { retryAction = this.Retry };
        }
    }
}