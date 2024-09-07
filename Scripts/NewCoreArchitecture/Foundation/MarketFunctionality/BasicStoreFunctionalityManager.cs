
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;

namespace Match3.Foundation.Base
{
    public abstract class BasicStoreFunctionalityManager : StoreFunctionalityManager
    {
        private string gameURL;
        private string developerId;
   
        public BasicStoreFunctionalityManager(string developerId)
        {
            SetGameURL(UnityEngine.Application.identifier);
            SetDeveloperId(developerId);
        }

        public void SetGameURL(string url)
        {
            this.gameURL = url;
        }

        public void SetDeveloperId(string id)
        {
            developerId = id;
        }
        
        public string GameURL()
        {
            return gameURL;
        }

        public string GamePageURL()
        {
            return StoreBaseURL() + gameURL;
        }

        public void RequestRating()
        {
            InternalRequestRatingFor(gameURL);
        }

        public void RequestRatingFor(string url)
        {
            InternalRequestRatingFor(url);
        }

        public void RequestVisitPage()
        {
            InternalRequestVisitPageFor(gameURL);
        }
        public void RequestVisitPageFor(string url)
        {
            InternalRequestVisitPageFor(url);
        }

        public void RequestVisitDeveloperPage()
        {
            InternalRequestVisitDeveloperPage(developerId);
        }

        public void RequestVisitDeveloperPage(string devId)
        {
            InternalRequestVisitDeveloperPage(devId);
        }

        protected abstract void InternalRequestRatingFor(string url);
        protected abstract void InternalRequestVisitPageFor(string url);
        protected abstract void InternalRequestVisitDeveloperPage(string devId);

        protected abstract string StoreBaseURL();

        public abstract long LastAvailableApplicationVersion();

        public abstract string StoreName();
    }
}
