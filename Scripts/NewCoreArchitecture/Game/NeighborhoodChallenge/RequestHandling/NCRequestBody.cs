
using Match3.Foundation.Base;
using Match3.Foundation.Base.ServiceLocating;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public class NCRequestBody : NiceJson.JsonObject
    {
        StoreFunctionalityManager storeFunctionalityManager;
        IUserProfile userProfile;

        public NCRequestBody()
        {
            this.storeFunctionalityManager = ServiceLocator.Find<StoreFunctionalityManager>();
            this.userProfile = ServiceLocator.Find<IUserProfile>();
        }

        public NCRequestBody AddChallengeName(string challengeName)
        {
            this.Add("challengeName", challengeName);
            return this;
        }

        public NCRequestBody AddMarket()
        {
            this.Add("market", storeFunctionalityManager.StoreName());
            return this;
        }

        public NCRequestBody AddEnvironment()
        {
            this.Add("env", "prod");
            return this;
        }

        public NCRequestBody AddPackageName()
        {
            this.Add("packageName", UnityEngine.Application.identifier);
            return this;
        }

        public NCRequestBody AddPlayerId()
        {
            this.Add("globalUniqueId", userProfile.GlobalUserId);
            return this;
        }

        public NCRequestBody Add<T>(string key, T value)
        {
            switch(value)
            {
                case string _val:
                    base.Add(key, _val);
                    break;
                case int _val:
                    base.Add(key, _val);
                    break;
                case float _val:
                    base.Add(key, _val);
                    break;

                default:
                    throw new System.Exception($"Value of type {typeof(T)} is not supported yet");

            }

            return this;
        }


    }
}