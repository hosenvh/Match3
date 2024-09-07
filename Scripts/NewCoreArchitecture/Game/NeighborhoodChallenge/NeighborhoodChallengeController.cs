using Match3.Game.NeighborhoodChallenge.RequestHandling;
using Match3.Presentation.TransitionEffects;

namespace Match3.Game.NeighborhoodChallenge
{
    public class NeighborhoodChallengeController 
    {
        protected NeighborhoodChallengeManager manager;

        protected GameTransitionManager transitionManager;

        public NeighborhoodChallengeController(NeighborhoodChallengeManager manager, GameTransitionManager transitionManager)
        {
            this.manager = manager;
            this.transitionManager = transitionManager;
        }

        public NeighborhoodChallengeManager Manager()
        {
            return manager;
        }

        protected NCFailureResult AddMapExitTo(NCFailureResult result)
        {
            if (result is BaseFailureResult baseResult)
                if(transitionManager.IsInMap() == false)
                    baseResult.confirmAction = transitionManager.GoToLastMap<DarkInTransitionEffect>;

            return result;
        }
    }
}