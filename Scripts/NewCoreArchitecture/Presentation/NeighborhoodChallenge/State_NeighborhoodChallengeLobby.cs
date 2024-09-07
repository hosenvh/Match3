using System;
using System.Collections;
using System.Collections.Generic;
using Match3;
using Match3.Data.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.MainShop;
using Match3.Game.NeighborhoodChallenge;
using Match3.Game.ShopManagement;
using Match3.Main;
using Match3.Presentation;
using Match3.Presentation.HUD;
using Match3.Presentation.NeighborhoodChallenge;
using Match3.Presentation.TransitionEffects;
using RTLTMPro;
using SeganX;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public struct NeighborhoodChallengeLobbyOpenedEvent : GameEvent { }

public enum NeighborhoodChallengeLobbyResultMode
{
    ClaimReward,
    GoToNextChallenge
}

public class State_NeighborhoodChallengeLobby : GameState, EventListener
{


    [Serializable]
    private struct GiftBoxRanking
    {
        public Sprite sprite ;
        public int startRank;
        public int endRank;

        public GiftBoxRanking(Sprite sprite, int startRank, int endRank)
        {
            this.sprite = sprite;
            this.startRank = startRank;
            this.endRank = endRank;
        }
    }
    
    // -------------------------------------------- Public  / Serialized Fields -------------------------------------------- \\ 

    public HudPresentationController hudPresentationController;
    public Canvas stateCanvas;

    [Space(15)]
    [SerializeField] private GameObject[] normalStateObjects = default;
    [SerializeField] private GameObject[] resultStateObjects = default;
    
    [Space(15)]
    [SerializeField] private GameObject claimRewardButton = default;
    [SerializeField] private GameObject goToNextChallengeButton = default;
    [SerializeField] private RectTransform userInfoRectTransform = default;
    [SerializeField] private RectTransform ranksViewPortRectTransform = default;
    [SerializeField] private Vector3 userInfoPositionForNormal = default;
    [SerializeField] private Vector3 userInfoPositionForResult = default;
    [SerializeField] private float ranksViewPortHeightForResult = default;
    [SerializeField] private float ranksViewPortHeightForNormal = default;
    

    [Space(15)]
    [SerializeField] private NeighborhoodChallengeRewardInfoMiniBoxController miniRewardBoxPresenter = default;
    [SerializeField] private Transform contentParent = default;
    [SerializeField] private GameObject separator = default;
    [SerializeField] private LeaderboardScoreBoxPresentationController scorePresenter = default;

    [Space(15)]
    [SerializeField] private RTLTextMeshPro timerText = default;

    [Space(15)]
    [SerializeField] private RTLTextMeshPro userRankText = default;
    [SerializeField] private RTLTextMeshPro userNameText = default;
    [SerializeField] private RTLTextMeshPro userScoreText = default;
    [SerializeField] private Image userGiftBoxImage = default;
    [SerializeField] private RectTransform userInfoLayoutRectTransform = default;

    [Space(15)] 
    [SerializeField] private Sprite doubleBombBoosterIcon = default;
    [SerializeField] private Sprite rainbowBoosterIcon = default;
    [SerializeField] private Sprite tntRainbowBoosterIcon = default;
    [SerializeField] private Sprite coinIcon = default;

    [Space(15)] 
    [SerializeField] private Sprite firstPlaceRankIcon = default;
    [SerializeField] private Sprite secondPlaceRankIcon = default;
    [SerializeField] private Sprite thirdPlaceRankIcon = default;
    [SerializeField] private Sprite otherPlaceRankIcon = default;
    
    [SerializeField] private CounterUI coinCounter = default;
    [SerializeField] private TimedCounterUI ticketCounter = default;

    [Space(15)] 
    [SerializeField] private GiftBoxRanking[] giftBoxRankings = default;
    
    
    // -------------------------------------------- Private Fields -------------------------------------------- \\

    private List<LeaderboardScoreBoxPresentationController> scoreBoxes = new List<LeaderboardScoreBoxPresentationController>();

    private NCUserInfo userInfo;
    
    private DateTime challengeEndTime;

    private Action<int, int> ticketPresneterUpdateAmountCache;
    NCTicket ticket;

    ChallengeData currentChallengeData;

    private Action onResultActionButtonPressed;

    // ======================================================================================================= \\

    
    
#if Test
    protected override void Start()
    {
        var data1 = new LeaderboardRanking {username = "sepehr", rank = 1, score = 10000};
        var data2 = new LeaderboardRanking {username = "ali", rank = 2, score = 9999};
        var data3 = new LeaderboardRanking {username = "ali2", rank = 3, score = 9998};
        var data4 = new LeaderboardRanking {username = "mojtaba", rank = 4, score = 9990};
        var data5 = new LeaderboardRanking {username = "ghoolam021", rank = 5, score = 10};
        var fakeData = new[] {data1, data2, data3, data4, data5};

        var rewardCoin = new CoinReward(500);
        var rewardRainbow = new RainbowBoosterReward(3);
        var rewardDoubleBomb = new DoubleBombBoosterReward(5);
        var rewardTntRainbow = new TntRainbowBoosterReward(1);
        var fakeReward = new List<Reward>() {rewardCoin, rewardRainbow, rewardDoubleBomb, rewardTntRainbow};
        
        var challengeReward1 = new ChallengeReward(fakeReward, 1, 1);
        var challengeReward2 = new ChallengeReward(fakeReward, 2, 2);
        var challengeReward3 = new ChallengeReward(fakeReward, 3, 3);
        var challengeReward4 = new ChallengeReward(fakeReward, 4, 50);
        var challengeRewards =
 new ChallengeReward[] {challengeReward1, challengeReward2, challengeReward3, challengeReward4 };

        var challengeExpTime = DateTime.UtcNow.AddDays(1);

        var uInfo = new UserInfo();
        uInfo.SetScore(1000);
        uInfo.SetName("sepehr");
        uInfo.SetRank(5);

        var challengeData = new ChallengeData
            {name = "aa", rewards = challengeRewards, endTime = DateTime.Now.AddDays(2), startTime = DateTime.Now};

        FillLeaderboard(fakeData, challengeData, uInfo);

        base.Start();
    }

#endif


    public State_NeighborhoodChallengeLobby Setup(NCInsideLobbyController lobbyController)
    {
        ServiceLocator.Find<EventManager>().Register(this);
        
        coinCounter.Setup(() => gameManager.profiler.CoinCount);

        ticket = lobbyController.Manager().Ticket();
        ticketCounter.Setup(
            () => ticket.CurrentValue(),
            () => (int)ticket.RemainingSecondsToNextRegeneration(),
            () => !ticket.IsFull());

        ticketPresneterUpdateAmountCache = (p,c) => ticketCounter.UpdateAmount();
        ticket.OnValueChanged += ticketPresneterUpdateAmountCache;

        lobbyController.Manager().Replace<LobbyControllerPortImp>(new LobbyControllerPortImp(lobbyController.Manager(), this));

        lobbyController.SetupLoby();

        ServiceLocator.Find<EventManager>().Propagate(new NeighborhoodChallengeLobbyOpenedEvent(), this);

        return this;
    }

    public void ShowTutorial()
    {
        ServiceLocator.Find<NeighborhoodChallengeTutorialManager>().Replay();
    }

    [UnityEngine.ContextMenu("Set To Normal State")]
    public void ChangeStateToNormal()
    {
        SetActiveNormalStateObjects(true);
        SetActiveResultStateObjects(false);
        
        ranksViewPortRectTransform.SetAnchordHeight(ranksViewPortHeightForNormal);
        userInfoRectTransform.anchoredPosition = userInfoPositionForNormal;
    }
    
    
    public void ChangeStateToResult(NeighborhoodChallengeLobbyResultMode mode, Action onActionButtonPressed)
    {
        SetActiveNormalStateObjects(false);
        SetActiveResultStateObjects(true);
        
        ranksViewPortRectTransform.SetAnchordHeight(ranksViewPortHeightForResult);
        userInfoRectTransform.anchoredPosition = userInfoPositionForResult;

        if (mode == NeighborhoodChallengeLobbyResultMode.ClaimReward)
        {
            claimRewardButton.SetActive(true);
            goToNextChallengeButton.SetActive(false);
        }
        else if (mode == NeighborhoodChallengeLobbyResultMode.GoToNextChallenge)
        {
            claimRewardButton.SetActive(false);
            goToNextChallengeButton.SetActive(true);
        }

        onResultActionButtonPressed = onActionButtonPressed;
    }

    public void ResultActionButtonPressed()
    {
        onResultActionButtonPressed();
    }


    public void EnterLevel()
    {
        var manager = ServiceLocator.Find<NeighborhoodChallengeManager>();

        if (manager.Ticket().CurrentValue() > 0)
        {
            gameManager.OpenPopup<Popup_LevelInfo>().Setup(
                manager.CurrentLevel(),
                -1,
                UpdateGUI,
                () =>
                {
                    gameManager.fxPresenter.PlayClickAudio();
                    manager.EnterLevel();
                });
        }
        else
            gameManager.OpenPopup<Popup_TicketRefill>().Setup(UpdateGUI);
    }


    public void FillLeaderboard(LeaderboardRanking[] rankings, ChallengeData challengeData, NCUserInfo userInfo)
    {
        
        // TODO: REFACTOR THIS.
        // ------------------------------------ CLEARING ----------------------------
        scoreBoxes.Clear();

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        // --------------------------------------------------------------------------
        
        
        currentChallengeData = challengeData;
        this.userInfo = userInfo;
        
        FillUserRankPresentation(userInfo);
        
        challengeEndTime = challengeData.endTime;

        StartCoroutine(OneSeceondUpdate(new WaitForSeconds(1)));

        for (var i = 0; i <= rankings.Length - 1; ++i)
        {
            GenerateScoreBox(rankings[i], i != rankings.Length - 1);
        }
    }
    
    
    
    public void UpdateUsername(string newName)
    {
        userNameText.text = newName;
        if (userInfo.Rank > 100) return;
        scoreBoxes[userInfo.Rank-1].SetUsername(newName);
        
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(userInfoLayoutRectTransform);
    }
    

    public void OpenGiftsInfoPopup()
    {   
        gameManager.OpenPopup<Popup_NeighborhoodChallengeGiftsInfo>().Setup(currentChallengeData.rankingRewards);
    }
    
    
    public void TryRefillTickets()
    {
        global::Base.gameManager.fxPresenter.PlayClickAudio();
        global::Base.gameManager.OpenPopup<Popup_TicketRefill>().Setup(UpdateGUI);
    }

    
    public void TryPurchaseCoin()
    {
        global::Base.gameManager.fxPresenter.PlayClickAudio();
        AnalyticsManager.SendEvent(new AnalyticsData_Map_HomeTap("tap coin icon"));
        global::Base.gameManager.shopCreator.TrySetupMainShop("lobby", UpdateGUI);
    }

    
    public void OpenUserGiftBox()
    {
        OpenRewardBox(userInfo.Rank, userGiftBoxImage.transform.position);
    }
    
    
    public void OpenRewardBox(int rank, Vector3 position)
    {
        var cRewards = currentChallengeData.rankingRewards;

        for (var i = 0; i <= cRewards.Count - 1; ++i)
        {
            if (rank < cRewards[i].start || (rank > cRewards[i].end && cRewards[i].end != -1)) continue;

            FillRewardBox(cRewards[i], position);
            
            break;
        }
    }
    
    
    
    private void SetActiveNormalStateObjects(bool active)
    {
        foreach (var ob in normalStateObjects)
        {
            ob.SetActive(active);
        }
    }

    private void SetActiveResultStateObjects(bool active)
    {
        foreach (var ob in resultStateObjects)
        {
            ob.SetActive(active);
        }
    }
    
    
    private void FillUserRankPresentation(NCUserInfo uInfo)
    {
        userRankText.text = (uInfo.Rank.ToString());
        userNameText.text = (uInfo.UserName);
        userScoreText.text = (uInfo.Score.ToString());
        userGiftBoxImage.sprite = GetProperGiftBoxSprite(uInfo.Rank);
        
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(userInfoLayoutRectTransform);
    }
    
    
    private void FillRewardBox(ChallengeRankingReward cReward, Vector3 position)
    {
        miniRewardBoxPresenter.gameObject.SetActive(true);
        miniRewardBoxPresenter.transform.position = position;
        miniRewardBoxPresenter.ResetBox();

        Sprite rewardIcon = null;
            
        for (var j = 0; j <= cReward.rewards.Count - 1; ++j)
        {
            var haveX = true;
            switch (cReward.rewards[j])
            {
                case CoinReward _:
                    rewardIcon = coinIcon;
                    haveX = false;
                    break;
                case DoubleBombBoosterReward _:
                    rewardIcon = doubleBombBoosterIcon;
                    break;
                case RainbowBoosterReward _:
                    rewardIcon = rainbowBoosterIcon;
                    break;
                case TntRainbowBoosterReward _:
                    rewardIcon = tntRainbowBoosterIcon;
                    break;
            }
                
            miniRewardBoxPresenter.AddReward(rewardIcon, cReward.rewards[j].count, haveX);
        }
    }
    

    private void GenerateScoreBox(LeaderboardRanking ranking, bool generateSeparator)
    {
        Sprite selectedRankIcon = SelectRankIcon(ranking.rank);
        Sprite selectedSpecialRewardIcon = SelectSpecialRewardIcon(ranking.rank);

        var generatedScoreBox = Instantiate(scorePresenter, contentParent);
        scoreBoxes.Add(generatedScoreBox); 
        generatedScoreBox.Fill(selectedRankIcon, selectedSpecialRewardIcon, GetProperGiftBoxSprite(ranking.rank), ranking.rank, ranking.username, ranking.score)
            .SetOnGiftBoxClickAction(OpenRewardBox);
        
        if (generateSeparator)
            Instantiate(separator, contentParent);
    }

    private Sprite SelectRankIcon(int rank)
    {
        return rank switch
        {
            1 => firstPlaceRankIcon,
            2 => secondPlaceRankIcon,
            3 => thirdPlaceRankIcon,
            _ => otherPlaceRankIcon
        };
    }

    private Sprite SelectSpecialRewardIcon(int rank)
    {
        return GetNeighborhoodChallengeServerConfig().GetSpecialRewardIconByRank(rank);

        NeighborhoodChallengeServerConfig GetNeighborhoodChallengeServerConfig() => ServiceLocator.Find<ServerConfigManager>().data.config.neighborhoodChallengeServerConfig;
    }

    private void UpdateTimer(string time)
    {
        timerText.text = time;
    }

    IEnumerator OneSeceondUpdate(WaitForSeconds waitForSeconds)
    {
        var leftTime = 0;
        while (true)
        {
            leftTime = (int) (challengeEndTime - DateTime.UtcNow).TotalSeconds;
            leftTime = Math.Max(0, leftTime);
            UpdateTimer(Utilities.GetLocalizedFormattedTime(leftTime));
            yield return waitForSeconds;
        }
    }


    private Sprite GetProperGiftBoxSprite(int rank)
    {
        bool IsRankAtThisGiftBoxRanking (GiftBoxRanking giftBoxRanking) => (rank >= giftBoxRanking.startRank &&
                             (rank <= giftBoxRanking.endRank || giftBoxRanking.endRank == -1));
        
        foreach (var giftBoxRanking in giftBoxRankings)
        {
            if(IsRankAtThisGiftBoxRanking(giftBoxRanking))
                return giftBoxRanking.sprite;
        }

        return giftBoxRankings[0].sprite;
    }
    
    
    public void UpdateGUI()
    {
        ticketCounter.UpdateAmount();
        coinCounter.UpdateAmount();
    }
    
    
    public override void Back()
    {
        gameManager.fxPresenter.PlayClickAudio();
        ServiceLocator.Find<GameTransitionManager>().GoToLastMap<DarkInTransitionEffect>();
    }

    private void OnDestroy()
    {
        ticket.OnValueChanged -= ticketPresneterUpdateAmountCache;
    }
    
    
    public void OnEvent(GameEvent evt, object sender)
    {
        if (evt is UpdateGUIEvent)
        {
            UpdateGUI();
        }
    }


    private void OnDisable()
    {
        ServiceLocator.Find<EventManager>().UnRegister(this);
    }


#if UNITY_EDITOR
    [UnityEngine.ContextMenu("Set To Result Claim State")]
    private void ChangeStateToResulClaim()
    {
        ChangeStateToResult(NeighborhoodChallengeLobbyResultMode.ClaimReward, delegate {  });
    }
    
    [UnityEngine.ContextMenu("Set To Result Next State")]
    private void ChangeStateToResulNextChallenge()
    {
        ChangeStateToResult(NeighborhoodChallengeLobbyResultMode.GoToNextChallenge, delegate {  });
    }
    
    #endif

    
}