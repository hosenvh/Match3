using I2.Loc;
using Match3.Foundation.Base;
using Match3.Foundation.Base.ServiceLocating;
using Medrick.Foundation.Base.PlatformFunctionality;
using UnityEngine;



namespace Match3.Presentation.MainMenu
{

    public class UpdateMenuButton : MainMenuButton
    {
        
        public static bool getFirstData = true;
        
        
        public override Transform Create(Transform buttonParent, Transform buttonControllerParent)
        {
            button.SetParent(buttonParent);
            CheckForUpdate();
            return button;
        }

        public override bool CreationCondition()
        {
            return true;
        }

        public override MainMenuButtonSetting GetSetting()
        {
            return MainMenuButtonsSettings.Update;
        }


        void CheckForUpdate()
        {
            button.gameObject.SetActive(false);
            var serverConfigRequest = new ServerConfigRequest();

            if (getFirstData)
            {
                serverConfigRequest.CheckForUpdate(UpdateCallback);
                getFirstData = false;
            }
            else
            {
                serverConfigRequest.UpdateServerTime((st) => { });
            }
        }

        void UpdateCallback(string updateState)
        {
            if (updateState.Equals("UPDATE"))
            {
                //Enable update button
                button.gameObject.SetActive(true);
                if (!IsGotUpdateMessage)
                {
                    IsGotUpdateMessage = true;
                    UpdateButtonCallback(false);
                }
            }
            else if (updateState.Equals("FORCE_UPDATE"))
            {
                UpdateButtonCallback(true);
            }
        }
        
        string GetUpdateMessageString()
        {
            return "UpdateMessage_" + ServiceLocator.Find<PlatformFunctionalityManager>().VersionCode(); 
        }
    
        public bool IsGotUpdateMessage
        {
            get { return PlayerPrefs.GetInt(GetUpdateMessageString(), 0) == 1; }
            set { PlayerPrefs.SetInt(GetUpdateMessageString(), value ? 1 : 0); }
        }
        
        public void UpdateButtonCallback(bool isForced)
        {
            var marketFunctionality = ServiceLocator.Find<StoreFunctionalityManager>();
            Base.gameManager.OpenPopup<Popup_Generic>().Setup(ScriptLocalization.Message.NewVersionAvailable, ScriptLocalization.UI_General.Update,
                delegate ()
                {
                    marketFunctionality.RequestVisitPage();
                }, !isForced);
        }
        
    }

}
