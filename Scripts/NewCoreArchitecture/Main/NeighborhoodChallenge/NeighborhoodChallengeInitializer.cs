using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;
using Match3.Game.NeighborhoodChallenge.RequestHandling;
using Match3.Presentation.NeighborhoodChallenge;

namespace Match3.Main.NeighborhoodChallenge
{
    public class NeighborhoodChallengeInitializer

    {
        public static void Initialize()
        {
            NeighborhoodChallengeManager manager = ServiceLocator.Find<NeighborhoodChallengeManager>();
            manager.Register(new LobbyEnteringPortImp(manager));
        }

    }
}