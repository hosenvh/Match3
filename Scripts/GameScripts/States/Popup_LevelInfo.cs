using System;
using System.Collections.Generic;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Initialization;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.LevelInfoAds;
using Match3.Presentation;
using Match3.Presentation.Hook.Containers;
using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine;
using UnityEngine.UI;


public class PopupLevelInfoOpenedEvent : GameEvent
{
    public Popup_LevelInfo levelInfoPopup;

    public PopupLevelInfoOpenedEvent(Popup_LevelInfo levelInfoPopup)
    {
        this.levelInfoPopup = levelInfoPopup;
    }
}

public class Popup_LevelInfo : GameState
{
    [Serializable]
    public struct DifficultyChangesEntry
    {
        public DifficultyLevel difficultyLevel;
        public Sprite sprite;
        public Color frameColor;
    }

    [SerializeField] private HookContainer hookContainer;
    
    [Space(10)]
    public Image backGroundImage;
    public Image frameImage;

    [SerializeField]
    private TextAdapter levelLabelText = null;
    [SerializeField]
    BoosterSelector[] boosterSelectors = default;

    public Transform goalsContainer;

    public LevelInfoGoalContainer levelInfoGoalContainerPrefab;

    public GoalTypePrefabDatabase goalTypePrefabDatabase;

    public List<DifficultyChangesEntry> difficultyChanges;

    Action onPurchaseFinish, onPlay;

    public HookContainer HookContainer => hookContainer;
  
    
    public void Setup(BoardConfig boardConfig, int levelIndex, Action onPurchaseFinish, Action onPlay)
    {
        this.onPurchaseFinish = onPurchaseFinish;
        this.onPlay = onPlay;
        AnalyticsManager.SendEvent(new AnalyticsData_LevelInfo_Open());
        switch(levelIndex)
        {
            case 0:
                gameManager.tutorialManager.CheckThenShowTutorial(79, 0, null);
                break;
            case 1:
                gameManager.tutorialManager.CheckThenShowTutorial(80, 0, null);
                break;
        }

        // TODO: Fix this when localizing hard-coded strings.
        if (levelIndex >= 0)
        {
            var label = ScriptLocalization.UI_LevelInfo.Level;
            switch (boardConfig.levelConfig.difficultyLevel)
            {
                case DifficultyLevel.Normal:
                    levelLabelText.SetText(string.Format(label, levelIndex + 1));
                    break;
                case DifficultyLevel.Hard:
                    levelLabelText.SetText(string.Format(label, levelIndex + 1) + ScriptLocalization.UI_LevelInfo.Hard);
                    break;
                case DifficultyLevel.VeryHard:
                    levelLabelText.SetText(string.Format(label, levelIndex + 1) + ScriptLocalization.UI_LevelInfo.VeryHard);
                    break;
            }
        }

        else
            switch (boardConfig.levelConfig.difficultyLevel)
            {
                case DifficultyLevel.Normal:
                    levelLabelText.SetText(ScriptLocalization.UI_LevelInfo.NormalLevel);
                    break;
                case DifficultyLevel.Hard:
                    levelLabelText.SetText(ScriptLocalization.UI_LevelInfo.HardLevel);
                    break;
                case DifficultyLevel.VeryHard:
                    levelLabelText.SetText(ScriptLocalization.UI_LevelInfo.VeryHardLevel);
                    break;
            }

        var changes = difficultyChanges.Find(e => e.difficultyLevel == boardConfig.levelConfig.difficultyLevel);
        backGroundImage.sprite = changes.sprite;
        frameImage.color = changes.frameColor;


        var goals = new GoalInfoExtaractor().Extract(boardConfig);

        foreach(var goal in goals)
        {
            var container = Instantiate(levelInfoGoalContainerPrefab, goalsContainer, false);
            container.gameObject.SetActive(true);
            container.Setup(goalTypePrefabDatabase.PrefabFor(goal.goalType), goal.goalAmount);
        }


        foreach (var item in boosterSelectors)
            item.Setup(UpdateGui);

        switch (levelIndex)
        {
            case 11:
                gameManager.tutorialManager.CheckThenShowTutorial(28, 0, null);
                break;
            case 15:
                gameManager.tutorialManager.CheckThenShowTutorial(29, 0, null);
                break;
            case 19:
                gameManager.tutorialManager.CheckThenShowTutorial(30, 0, null);
                break;
        }
        
        ServiceLocator.Find<EventManager>().Propagate(new PopupLevelInfoOpenedEvent(this), this);
    }

    public void OnPlayClick()
    {
        gameManager.tutorialManager.CheckAndHideTutorial(79);
        gameManager.tutorialManager.CheckAndHideTutorial(80);
        new LevelInfoRewaredAdsHandler().TryPlayRewardedAds(onComplete: onPlay);
    }

    public void UpdateGui()
    {
        foreach (var item in boosterSelectors)
            item.UpdateGUI();
        if (onPurchaseFinish != null)
            onPurchaseFinish();
    }

    public override void Back()
    {
        gameManager.fxPresenter.PlayClickAudio();
        AnalyticsManager.SendEvent(new AnalyticsData_LevelInfo_Close());
        base.Back();
    }
}
