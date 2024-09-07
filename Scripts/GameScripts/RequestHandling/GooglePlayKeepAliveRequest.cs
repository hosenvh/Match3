using Medrick.ServerCommunication.Basic.Application.Request;
using Medrick.ServerCommunication.Basic.Application.Response;
using Newtonsoft.Json;

namespace Match3
{
    public class GooglePlayKeepAliveRequestBody : IHttpRequestBody
    {
        [JsonProperty("market_name")]
        public string MarketName { get; }

        [JsonProperty("package_name")]
        public string PackageName { get; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; }

        [JsonProperty("sku")]
        public string Sku { get; }

        public GooglePlayKeepAliveRequestBody(string marketName, string packageName, string transactionId, string sku)
        {
            MarketName = marketName;
            PackageName = packageName;
            TransactionId = transactionId;
            Sku = sku;
        }
    }

    public class GooglePlayKeepAliveResponseBody : IHttpResponseBody
    {
        [JsonProperty("code")]
        public string Code { get; }

        [JsonProperty("detail")]
        public string Detail { get; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; }

        [JsonProperty("consumption_state")]
        public string ConsumptionState { get; }

        [JsonProperty("purchase_state")]
        public string PurchaseState { get; }

        [JsonProperty("developer_payload")]
        public string DeveloperPayload { get; }
    }
}