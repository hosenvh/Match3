using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using Match3.Main.Localization;
using Match3.Presentation;
using Match3.Presentation.LanguageSelection;
using SeganX;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Match3.Main
{
    public class LanguageSelectionTask : BasicTask
    { 
        
        private const string PrefabPath = "Prefabs/UI/Popup_LanguageSelection_withoutManager";

        private bool isFirstOpen = false;

        public LanguageSelectionTask(bool isFirstOpen)
        {
            this.isFirstOpen = isFirstOpen;
        }

        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            var shouldAskSelectLanguage = ServiceLocator.Find<PresentationElementActivationStateCenter>().
                IsActive(PresentationElement.LanguageSelection);

            if (shouldAskSelectLanguage && isFirstOpen)
            {
                GameLanguageHelper.SetGameLanguageAsSystemLanguage("en");
                OpenLanguageSelectionPopup().Setup(false, false,
                    onComplete);
            }
            else
                onComplete();
        }

        public Popup_LanguageSelection OpenLanguageSelectionPopup()
        {
            var popup = Resources.Load<Popup_LanguageSelection>(PrefabPath);
            var res = Object.Instantiate(popup.gameObject).GetComponent<Popup_LanguageSelection>();
            return res;
        }
    }
}


