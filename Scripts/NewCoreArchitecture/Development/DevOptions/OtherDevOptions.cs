using Firebase.Messaging;
using Match3.Development.Base;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.HUD;
using SeganX;
using UnityEngine;
using static Base;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Other", priority: 11)]
    public class OtherDevOptions : DevelopmentOptionsDefinition
    {
        [DevOption(commandName: "Scale Time")]
        public static void ScaleTime(float scale)
        {
            Time.timeScale = scale;
        }

        [DevOption(commandName: "Reset Time Scale")]
        public static void ResetTimeScale()
        {
            Time.timeScale = 1;
        }

        [DevOption(commandName: "Set Target FrameRate")]
        public static void SetTargetFrameRate(int target)
        {
            PrepareSoChangingFrameRateDoTakeEffect();
            UpdateTargetFrameRateToTargetValue();

            void PrepareSoChangingFrameRateDoTakeEffect()
            {
                #if UNITY_EDITOR
                QualitySettings.vSyncCount = 0;
                #endif
            }

            void UpdateTargetFrameRateToTargetValue() => Application.targetFrameRate = target;
        }

        [DevOption(commandName: "Check App Installed")]
        public static void CheckAppInstalled(string bundle)
        {
            Debug.Log($"App is Installed : {Utilities.IsApplicationInstalled(bundle)}");
        }

        [DevOption(commandName: "Rate Game")]
        public static void RateGame()
        {
            var marketFunctionality = ServiceLocator.Find<StoreFunctionalityManager>();
            marketFunctionality.RequestRating();
        }

        [DevOption(commandName: "Update Game")]
        public static void UpdateGame()
        {
            var marketFunctionality = ServiceLocator.Find<StoreFunctionalityManager>();
            marketFunctionality.RequestVisitPage();
        }

        [DevOption(commandName: "Subscribe To Firebase Dev Topic")]
        public static void SubscribeToDevelopmentFirebaseTopic()
        {
            FirebaseMessaging.SubscribeAsync("/topics/Developer");
        }

        [DevOption(commandName: "Open Social Post")]
        public static void OpenSocialPost()
        {
            var post = gameManager.socialAlbumController.GetAllPosts()[0];
            gameManager.OpenPopup<Popup_SocialAlbumPicturePresenter>().Setup(post);
        }

        [DevOption(commandName: "Open UpdateWelcome", shouldAutoClose: true)]
        public static void OpenUpdateWelcomePopup()
        {
            var hudPresentationController = Object.FindObjectOfType<HudPresentationController>();
            var eventManager = ServiceLocator.Find<EventManager>();
            gameManager.updateWelcomeController.OpenUpdateWelcomePopup(hudPresentationController, eventManager, onFinishClaimReward: delegate { });
        }
    }
}