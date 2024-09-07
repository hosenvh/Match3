using System;
using Match3.Game.NeighborhoodChallenge;
using Match3.Game.NeighborhoodChallenge.RequestHandling;
using Match3.Presentation.TransitionEffects;


namespace Match3.Presentation.NeighborhoodChallenge
{
    public class LobbyControllerPortImp : NCPresentationPort, LobbyControllerPort
    {
        State_NeighborhoodChallengeLobby state;


        public LobbyControllerPortImp(NeighborhoodChallengeManager challengeManager, State_NeighborhoodChallengeLobby state) : base(challengeManager)
        {
            this.state = state;
        }

        public void Wait()
        {
            BeginWaiting();
        }

        public void HandleError(NCFailureResult failureResult)
        {
            HandleGeneralError(failureResult);
        }

        public void ShowCurrentLeaderBoard(LeaderBoardData leaderBoardData, NCUserInfo userInfo)
        {
            EndWaiting();
            state.ChangeStateToNormal();
            state.FillLeaderboard(leaderBoardData.rankings.ToArray(), leaderBoardData.challengeData, userInfo);
        }

        public void ShowRewardClaimingFor(ChallengeRankingReward reward, LeaderBoardData leaderBoardData, NCUserInfo userInfo, Action onCompleted)
        {
            EndWaiting();

            state.ChangeStateToResult( NeighborhoodChallengeLobbyResultMode.ClaimReward, () =>
            {
                Base.gameManager.OpenPopup<Popup_ClaimReward>()
                    .Setup(reward.rewards.ToArray())
                    .OverrideHudControllerForDisappearingEffect(state.hudPresentationController)
                    .OverrideRenderingCamera(state.stateCanvas.worldCamera)
                    .OverrideOrderInLayer(state.stateCanvas.sortingOrder + 1).StartPresentingRewards();
                
                state.ChangeStateToResult(NeighborhoodChallengeLobbyResultMode.GoToNextChallenge, () =>
                {
                    var darkInTransitionEffect = new DarkInTransitionEffect();
                    darkInTransitionEffect.StartTransitionEffect(() =>
                    {
                        onCompleted.Invoke();
                    }, delegate {  });
                });
                
            });
            
            state.FillLeaderboard(leaderBoardData.rankings.ToArray(), leaderBoardData.challengeData, userInfo);
        }
    }
}