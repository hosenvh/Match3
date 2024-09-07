
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay;

namespace Match3.Game.NeighborhoodChallenge
{
    public class NeighborhoodChallengeLifeConsumer : LifeConsumer
    {
        public void ConsumeLife()
        {
            ServiceLocator.Find<NeighborhoodChallengeManager>().ConsumeLife();
        }
    }
}