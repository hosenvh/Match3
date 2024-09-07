using System.Threading;
using System.Threading.Tasks;
using I2.Loc;
using Match3.Presentation;
using SeganX;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Match3;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Utility.GolmoradLogging;


namespace DynamicSpecialOfferSpace
{

    public class DynamicSpecialOfferPopUp : MonoBehaviour
    {
//        private static string defaultPopUpSceneName = "sce_DynamicSpecialOfferPopUp";

        public static bool IsOpen = false;

        // ----------------------------------------------- Public Fields ----------------------------------------------- \\

        [Space(10)] 
        public Image featureArtImage;

        [Space(10)] 
        public LocalText titleText;
        public LocalText coinCountText;
        public LocalText[] powerUpsCountTexts = new LocalText[3];

        [Space(10)]
        public LocalText realPriceText;
        public LocalText discountPriceText;
        public LocalText offPercentText;

        [Space(10)]
        public LocalText compactDiscountText;
        public LocalizedStringTerm compactDiscountString;
        
        [Space(10)] 
        public Button purchaseButton;
        public Button closeButton;

        [Space(10)] 
        public LocalizedStringTerm countLocalizedString;

        // ============================================================================================================= \\

        public static async Task<bool> Spawn(CancellationToken ct, DynamicSpecialOfferPackage offer, string popupScene)
        {
            KeepAliveHelper.SetCurrentPage("map");
            var offerPopup = await OpenPopupScene(ct, popupScene);
            IsOpen = true;
            offerPopup.InitPopUp(offer);
            var result = await WaitForButtonOperation(ct, offerPopup);
            Utilities.UnloadScene(popupScene);
            IsOpen = false;
            return result;
        }
        
        
        private static async Task<DynamicSpecialOfferPopUp> OpenPopupScene(CancellationToken ct, string sceneName)
        {
            var offerPopup  = await Utilities.LoadScene<DynamicSpecialOfferPopUp>(ct, sceneName, LoadSceneMode.Additive);
            return offerPopup;
        }

        private static async Task<bool> WaitForButtonOperation(CancellationToken ct, DynamicSpecialOfferPopUp offerPopup)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                var linkedCt = linkedCts.Token;
                var selectButtonTask =
                    Utilities.WaitForSelectButton(linkedCt, offerPopup.purchaseButton, offerPopup.closeButton);
                var backPressTask = Utilities.WaitForBackPress(linkedCt);
                var taskResult = await Task.WhenAny(selectButtonTask, backPressTask);
                await taskResult;
                linkedCts.Cancel();
                return taskResult == selectButtonTask && selectButtonTask.Result == offerPopup.purchaseButton;
            }
            finally
            {
                linkedCts.Dispose();
            }
        }

        private void InitPopUp(DynamicSpecialOfferPackage offer)
        {
            if (titleText != null)
                titleText.SetText(offer.Title());


            foreach(var reward in offer.Rewards())
            {
                switch (reward)
                {
                    case CoinReward _:
                        coinCountText.SetText(string.Format(countLocalizedString, reward.count.ToString("N0")));
                        break;
                    case AllBoostersReward _:
                        for (int i = powerUpsCountTexts.Length - 1; i >= 0; i--)
                            powerUpsCountTexts[i].SetText(reward.count.ToString());
                        break;
                    default:
                        DebugPro.LogError<DynamicSpecialOfferLogTag>($"[DynamicSpecialOfferPopup] Reward of type {reward.GetType()} is not defined");
                        break;
                }
            }




            realPriceText.SetText(offer.DiscountInfo().BeforeDiscountPrice().FormatMoneyToString());
            discountPriceText.SetText(offer.Price().FormatMoneyToString());
            
            if(compactDiscountText!=null)
                compactDiscountText.SetText(string.Format(compactDiscountString, offer.Tag()));
            else
                offPercentText.SetText(offer.Tag());
        }
        
    }

}