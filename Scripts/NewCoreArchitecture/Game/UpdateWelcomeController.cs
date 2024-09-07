using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Data;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.PlayerStats;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.HUD;
using Match3.Presentation.UpdateWelcome;
using Medrick.Foundation.Base.PlatformFunctionality;
using UnityEngine;


namespace Match3.Game.UpdateWelcome
{

    public class UpdateWelcomeController
    {
        private const string LastVersionKey = "UpdateWelcomeLastVersionKey";

        private readonly int currentVersion;
        
        private int updateCoinRewardCount;
        private List<ChangeLog> changeLogs;

        private bool isFirstOpen;
        
        private int LastCheckedVersion
        {
            get => PlayerPrefs.GetInt(LastVersionKey, 0);
            set => PlayerPrefs.SetInt(LastVersionKey, value);
        }

        public UpdateWelcomeController(bool isFirstOpen)
        {
            this.isFirstOpen = isFirstOpen;
            
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
            currentVersion = ServiceLocator.Find<PlatformFunctionalityManager>().VersionCode();

            if (LastCheckedVersion == 0 && isFirstOpen)
                UpdateLastCheckedVersion();
        }

        
        public void SetChangeLogs(List<ChangeLog> changeLogs)
        {
            this.changeLogs = changeLogs;
        }

        public void SetUpdateCoinReward(int count)
        {
            updateCoinRewardCount = count;
        }

        public bool ShouldOpenUpdateWelcome()
        {
            return !isFirstOpen && LastCheckedVersion < currentVersion;
        }

        public void OpenUpdateWelcomePopup(HudPresentationController hudPresentationController, EventManager eventManager, Action onFinishClaimReward)
        {
            var updatePopup = Base.gameManager.OpenPopup<Popup_UpdateWelcome>();
            updatePopup.SetOnFinishClaimReward(onFinishClaimReward);
            updatePopup.Setup(hudPresentationController, eventManager, this,
                changeLogs.LastOne().changeLog, updateCoinRewardCount);
        }

        public void UpdateLastCheckedVersion()
        {
            LastCheckedVersion = currentVersion;
        }
        
    }
    
}


