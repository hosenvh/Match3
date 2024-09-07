using System;
using I2.Loc;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Foundation.Base.NotificationService;
using Match3.Presentation.TransitionEffects;
using SeganX;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.LanguageSelection
{

    public class Popup_LanguageSelection : GameState
    {
        
        [Serializable]
        public class LanguageBlock
        {
            public string language;
            public Toggle toggle;
        }

        public LanguageBlock[] LanguageBlocks;
        public GameObject closeButton;

        private Action onConfirm;
        private string selectedLanguage;
        private static string activeLanguage;

        private bool preventOfBackClose;
        
        
        public void Setup(bool reloadMapState, bool activeCloseButton, Action onConfirm)
        {
            this.onConfirm = onConfirm;
            
            selectedLanguage = LocalizationManager.CurrentLanguage;
            if (string.IsNullOrEmpty(activeLanguage))
                activeLanguage = selectedLanguage;
            
            closeButton.SetActive(activeCloseButton);
            preventOfBackClose = !activeCloseButton;
            
            foreach (var languageBlock in LanguageBlocks)
            {
                if (selectedLanguage == languageBlock.language)
                {
                    languageBlock.toggle.isOn = true;
                }
                
                languageBlock.toggle.onValueChanged.AddListener((isTick) =>
                {
                    if (isTick)
                    {
                        if (reloadMapState)
                        {
                            ChangeLanguageAndRefreshMap(languageBlock.language);
                            gameManager.OpenPopup<Popup_LanguageSelection>().Setup(reloadMapState, activeCloseButton, onConfirm);
                        }
                        else
                            ChangeLanguage(languageBlock.language);
                    }
                });
            }
        }

        public override void Back()
        {
            if (preventOfBackClose) return;
            base.Back();
            if(selectedLanguage != activeLanguage)
                ChangeLanguageAndRefreshMap(activeLanguage);
        }

        public void ConfirmAndClose()
        {
            activeLanguage = "";
            onConfirm?.Invoke();
            UpdateScheduledNotificationsLanguage();
            base.Back();
        }
        
        public void ConfirmAndCloseWithDestroy()
        {
            onConfirm?.Invoke();
            Destroy(gameObject);
        }


        private void UpdateScheduledNotificationsLanguage()
        {
            ServiceLocator.Find<NotificationService>().ReScheduleWaitingNotifications();
        }
        
        
        private void ChangeLanguage(string language)
        {
            LocalizationManager.CurrentLanguage = language;
        }

        private void ChangeLanguageAndRefreshMap(string language)
        {
            gameManager.SetLanguageChanging(true);
            ChangeLanguage(language);
            ReOpenMapState();
        }

        private void ReOpenMapState()
        {
            ServiceLocator.Find<GameTransitionManager>().GoToLastMap<DarkInTransitionEffect>();
        }
        
        
    }
    
}


