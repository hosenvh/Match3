using System;
using System.Collections.Generic;
using UnityEngine;



public class TutorialManager : Base
{
    public List<TutorialConfig> tutorialConfigs;
    int currentShowingTutorialIndex = -1;

    private Action onComplete;
    
    public bool CheckThenShowTutorial(int tutorialIndex, float delay, Action onHide, Action onComplete = null)
    {
        TutorialConfig tutorialConfig = tutorialConfigs[tutorialIndex];
        if (!IsTutorialShowed(tutorialIndex))
        {
            DelayCall(delay, () => { ShowTutorial(tutorialIndex, onHide, onComplete); });
            SetTutorialShowed(tutorialIndex);
            return true;
        }
        return false;
    }

    private void ShowTutorial(int tutorialIndex, Action onHide, Action onComplete)
    {
        // Debug.LogWarning("****** "+tutorialConfigs[tutorialIndex].analyticId + " **** " + tutorialIndex);
        if (tutorialConfigs[tutorialIndex].analyticId == 1)
            AnalyticsManager.SendEvent(new AnalyticsData_Tutorial_Start());
        else if (tutorialConfigs[tutorialIndex].analyticId == 21)
            AnalyticsManager.SendEvent(new AnalyticsData_Tutorial_Finish());

        if (tutorialConfigs[tutorialIndex].analyticId >= 0)
            AnalyticsManager.SendEvent(new AnalyticsData_Tutorial_Step(tutorialConfigs[tutorialIndex].analyticId));
        currentShowingTutorialIndex = tutorialIndex;
        var tutorialPopup = gameManager.OpenPopup<Popup_Tutorial>();
        tutorialPopup.OnCloseEvent += onComplete;
        tutorialPopup.Setup(tutorialConfigs[tutorialIndex], () =>
        {
            if (onHide != null)
                onHide();
            CheckAndHideTutorial(tutorialIndex);
        });
    }

    public bool CheckAndHideTutorial(int tutorialIndex)
    {
        if (currentShowingTutorialIndex == tutorialIndex)
        {
            currentShowingTutorialIndex = -1;
            gameManager.ClosePopup();
            return true;
        }
        return false;
    }

    public string TutorialShowedString(int tutorialIndex)
    {
        return "Tutorial_" + tutorialIndex;
    }
    public bool IsTutorialShowed(int tutorialIndex)
    {
        return PlayerPrefs.GetInt(TutorialShowedString(tutorialIndex), 0) == 1;
    }
    void SetTutorialShowed(int tutorialIndex)
    {
        PlayerPrefs.SetInt(TutorialShowedString(tutorialIndex), 1);
    }

    public void SetTutorialState(int tutorialIndex, int state)
    {
        PlayerPrefs.SetInt(TutorialShowedString(tutorialIndex), state);
    }
}