using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge.RequestHandling;
using System;
using Match3.UserManagement.Main;
using Match3.UserManagement.ProfileName;


namespace Match3.Game.NeighborhoodChallenge
{

    class UserNotRegisteredResult : NCFailureResult
    {

    }

    public interface NCLobbyEnteringPort : NeighborhoodChallengePort
    {
        void OpenLobby();
    }

    public class NCLobbyEnteringController : NeighborhoodChallengeController
    {

        AllChallengesRequestHandler allChallengesRequestHandler = new AllChallengesRequestHandler();
        PlayerRelevantChallengesRequestHandler playerRelevantChallengeRequestHandler = new PlayerRelevantChallengesRequestHandler();
        ChallengeRegisteringRequestHandler challengeRegisteringRequestHandler = new ChallengeRegisteringRequestHandler();

        private UserProfileNameManager UserProfileNameManager => ServiceLocator.Find<UserManagementService>().UserProfileNameManager;

        public NCLobbyEnteringController(NeighborhoodChallengeManager manager, GameTransitionManager transitionManager) : base(manager, transitionManager)
        {
        }


        public void EnterLobby()
        {
            var port = manager.GetPort<NCLobbyEnteringPort>();
            port.Wait();

            manager.PrepareLobby();
            Action<ChallengeRegisteringResult> challengeRegisteringRSuccessAction = (r) =>
            {
                manager.SetPlayerActiveChallenge(r.challengeName);
                port.OpenLobby();
            };

            Action<PlayerRelevantChallengesResult> playerRelevantChallengeSuccessAction = (r) =>
            {
                manager.SetNotClaimedChallengesNames(r.notClaimedChallengesName);
                // NOTE: In some situations server may say a player's challenge is active but in reality it's not active.
                // That's why it is checking manager.HasActiveChallenge
                if (r.currentActiveChallengesName.Count > 0 && manager.HasActiveChallenge(r.currentActiveChallengesName[0]))
                {
                    manager.SetPlayerActiveChallenge(r.currentActiveChallengesName[0]);
                    port.OpenLobby();
                }
                else
                {
                    challengeRegisteringRequestHandler.Request(
                        manager.ActiveChallenges()[0].name,
                        challengeRegisteringRSuccessAction,
                        (e) => port.HandleError(AddMapExitTo(e)));
                }
            };

            Action<AllChallengesResult> allChallengesRequestSuccessAction = (r) =>
            {
                if (r.challengesData.Count < 1)
                {
                    port.HandleError(AddMapExitTo(new NoChallengeIsActiveForGame()));
                }
                else
                {
                    manager.SetActiveChallenges(r.challengesData);
                    playerRelevantChallengeRequestHandler.Request(
                        playerRelevantChallengeSuccessAction,
                        (e) => port.HandleError(AddMapExitTo(e)));
                }
            };

            if (UserProfileNameManager.IsProfileNameEverSet())
                ContinueLobbyEntering();
            else
                UserProfileNameManager.AskForUserProfileName(onSubmit: ContinueLobbyEntering, onCanceled: delegate { });

            void ContinueLobbyEntering()
            {
                allChallengesRequestHandler.Request(activeChallengesOnly: true,
                                                    allChallengesRequestSuccessAction,
                                                    onFailure: (e) => port.HandleError(AddMapExitTo(e)));
            }
        }
    }
}