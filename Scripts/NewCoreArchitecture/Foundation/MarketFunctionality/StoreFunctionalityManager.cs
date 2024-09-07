using Match3.Foundation.Base.ServiceLocating;


namespace Match3.Foundation.Base
{
    public interface StoreFunctionalityManager : Service
    {
        // TODO: This must change to SetGameIdentifier
        void SetGameURL(string url);

        string GamePageURL();
        
        void RequestRating();
        void RequestRatingFor(string url);

        void RequestVisitPage();
        void RequestVisitPageFor(string url);

        void RequestVisitDeveloperPage();
        void RequestVisitDeveloperPage(string devId);

        long LastAvailableApplicationVersion();

        string StoreName();
    }
}