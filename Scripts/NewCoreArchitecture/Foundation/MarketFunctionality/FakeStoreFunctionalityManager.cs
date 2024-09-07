
using Match3.Foundation.Base;

namespace Medrick.Foundation.Base.MarketFunctionality
{
    public class FakeStoreFunctionalityManager : BasicStoreFunctionalityManager
    {
        
        public FakeStoreFunctionalityManager(string developerId) : base(developerId)
        {
        }
        

        public override long LastAvailableApplicationVersion()
        {
            return 0;
        }

        public override string StoreName()
        {
            return "Fake";
        }

        protected override void InternalRequestRatingFor(string url)
        {
            
        }

        protected override void InternalRequestVisitPageFor(string url)
        {
            
        }
        
        protected override void InternalRequestVisitDeveloperPage(string devId)
        {
            throw new System.NotImplementedException();
        }

        protected override string StoreBaseURL()
        {
            return "";
        }
    }
}