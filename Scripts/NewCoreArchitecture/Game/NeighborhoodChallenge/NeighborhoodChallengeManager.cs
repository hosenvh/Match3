using System.Collections.Generic;
using KitchenParadise.Foundation.Base.TimeManagement;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge.RequestHandling;
using Match3.Utility.GolmoradLogging;
using UnityEngine;


namespace Match3.Game.NeighborhoodChallenge
{
    public class NeighborhoodChallengeUnlockedEvent : GameEvent
    {

    }

    public interface NeighborhoodChallengePort
    {
        void Wait();
        void HandleError(NCFailureResult failureResult);
    }

    public class NeighborhoodChallengeManager : Service
    {

        NCUserInfo userInfo = new NCUserInfo();
        List<ChallengeData> activeChallenges = new List<ChallengeData>();
        List<string> notClaimedChallenges = new List<string>();

        List<NeighborhoodChallengeController> controllers = new List<NeighborhoodChallengeController>();
        List<NeighborhoodChallengePort> ports = new List<NeighborhoodChallengePort>();


        private NCTicket ticket;
        private NeighborhoodChallengeLevelSelector levelSelector;
        private NeighborhoodChallengeActivationPolicy activationPolicy;

        public NeighborhoodChallengeLevelSelector LevelSelector => levelSelector;
        

        bool isUnlocked = false;

        public NeighborhoodChallengeManager()
        {
            levelSelector = new NeighborhoodChallengeLevelSelector();
            ticket = CreateTicket(); 

            var transitionManager = ServiceLocator.Find<GameTransitionManager>();

            controllers.Add(new NCInsideLobbyController(this, transitionManager));
            controllers.Add(new NCLobbyEnteringController(this, transitionManager));
            controllers.Add(new NCLevelScoringController(this, transitionManager));
            controllers.Add(new NCLevelScoringController(this, transitionManager));
            controllers.Add(new NCLevelChangingController(this, levelSelector.ForceSelectNewLevel));

            activationPolicy = new NeighborhoodChallengeActivationPolicy(Unlock);
        }

        private NCTicket CreateTicket()
        {
            var ticket = new NCTicket();
            ServiceLocator.Find<UpdateManager>().RegisterUpdatable(ticket, Base.gameManager.mainTimeChannel);
            ServiceLocator.Find<ConfigurationManager>().Configure(ticket);
            ticket.Load();
            return ticket;
        }

        private void Unlock()
        {
            ServiceLocator.Find<EventManager>().Propagate(new NeighborhoodChallengeUnlockedEvent(), this);
            isUnlocked = true;
        }

        public void Register(NeighborhoodChallengePort port)
        {
            ports.Add(port);
        }

        public void Replace<T>(NeighborhoodChallengePort port) where T : NeighborhoodChallengePort
        {
            ports.RemoveAll(p => p is T);
            Register(port);
        }


        public T GetPort<T>() where T : NeighborhoodChallengePort
        {
            return (T) ports.Find(p => p is T);
        }

        public void SetActiveChallenges(List<ChallengeData> challenges)
        {
            activeChallenges = challenges;

            foreach(var challenge in challenges)
            {
                DebugPro.LogInfo<NeighborHoodChallengeLogTag>($"Challenge {challenge.name}");
            }
        }

        public void SetNotClaimedChallengesNames(List<string> challenges)
        {
            notClaimedChallenges = challenges;
        }


        public string NotClaimedChallengeName()
        {
            return notClaimedChallenges[0];
        }

        public bool UserHasNotClaimedReward()
        {
            return notClaimedChallenges.Count > 0;
        }

        public void MarkAsClaimed(string challengeName)
        {
            notClaimedChallenges.Remove(challengeName);
        }

        public T GetController<T>() where T : NeighborhoodChallengeController
        {
            return (T)controllers.Find(s => s is T);
        }
  

        public bool HasActiveChallenge(string challengeMan)
        {
            return FindActiveChallengeWithName(challengeMan) != null;
        }


        public void ConsumeLife()
        {
            ticket.Consume();
        }

        public void HandleLevelWinning()
        {
            levelSelector.ForceSelectNewLevel();
            ticket.AddValue(1);
        }

        public void PrepareLobby()
        {
            levelSelector.InitializeSelection();
        }

        public NCTicket Ticket()
        {
            return ticket;
        }

        public NCUserInfo UserInfo()
        {
            return userInfo;
        }

        public void SetPlayerActiveChallenge(string challengeName)
        {
            UserInfo().SetActiveChallenge(FindActiveChallengeWithName(challengeName));
        }

        public BoardConfig CurrentLevel()
        {
            return Base.gameManager.levelManager.GetLevelConfig(CurrentLevelIndex());
        }

        private int CurrentLevelIndex()
        {
            return Mathf.Min(Base.gameManager.levelManager.TotalLevels()-1, levelSelector.SelectedLevel());
        }

        public void ReloadCurrentLevel()
        {
            ServiceLocator.Find<GameTransitionManager>().GoToNCLevel(CurrentLevel(), CurrentLevelIndex());
        }

        public void EnterLevel()
        {
            ServiceLocator.Find<GameTransitionManager>().GoToNCLevel(CurrentLevel(), CurrentLevelIndex());
        }

        ChallengeData FindActiveChallengeWithName(string challengeName)
        {
            return activeChallenges.Find(c => c.name.Equals(challengeName));
        }


        public List<ChallengeData> ActiveChallenges()
        {
            return activeChallenges;
        }


        public bool IsUnlocked()
        {
            return isUnlocked;
        }

        public bool IsEnabled()
        {
            return activationPolicy.IsEnabled();
        }
    }
}