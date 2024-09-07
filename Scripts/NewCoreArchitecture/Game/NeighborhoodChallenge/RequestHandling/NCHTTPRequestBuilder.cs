
using Match3.Foundation.Base;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Network;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class NCHTTPRequestBuilder : HTTPRequestBuilder
    {
        StoreFunctionalityManager storeFunctionalityManager;
        IUserProfile userProfile;

        public NCHTTPRequestBuilder()
        {
            this.storeFunctionalityManager = ServiceLocator.Find< StoreFunctionalityManager>();
            this.userProfile = ServiceLocator.Find<IUserProfile>();
        }

        public NCHTTPRequestBuilder AddEnvironment()
        {
            this.AddParameters("env", "prod");
            return this;
        }

        public NCHTTPRequestBuilder AddMarket()
        {
            this.AddParameters("market", storeFunctionalityManager.StoreName());
            return this;
        }

        public NCHTTPRequestBuilder AddPlayerID()
        {
            this.AddParameters("globalUniqueId", userProfile.GlobalUserId);
            return this;
        }

        public NCHTTPRequestBuilder AddPackageName()
        {
            this.AddParameters("packageName", UnityEngine.Application.identifier);
            return this;
        }

        public NCHTTPRequestBuilder AddChallengeName(string challengeName)
        {
            this.AddParameters("challengeName", challengeName);
            return this;
        }

        public NCHTTPRequestBuilder SetBody(NCRequestBody body)
        {
            this.SetBody(body.ToJsonString());
            return this;
        }
    }
}