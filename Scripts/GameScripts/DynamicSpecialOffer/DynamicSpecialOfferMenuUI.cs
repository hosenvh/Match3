using System;
using I2.Loc;
using SeganX;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Match3;
using Match3.Utility.GolmoradLogging;
using static Match3.Presentation.ShopManagement.ShopPopupCreator;


namespace DynamicSpecialOfferSpace
{

    public class DynamicSpecialOfferMenuUI : MonoBehaviour
    {

        // ---------------------------------------- Public Fields ---------------------------------------- \\
        
        [SerializeField] private GameObject offerButtonObject = default;
        [SerializeField] private Button offerButton = default;
        [SerializeField] private LocalText offerTimerText = default;
        [SerializeField] private LocalText offAmountText= default;

        // ---------------------------------------- Private Fields ---------------------------------------- \\

        private bool inited = false;
        
        private UnityAction onPurchaseButtonSelected;
        private DynamicSpecialOfferPackage offer;
        private Popup_DynamicSpecialOffer openedDynamicSpecialOfferPopup;
        // ================================================================================================ \\


        private void Awake()
        {
            offerButton.onClick.AddListener(UpdateStorePackagesAndOpenSpecialOfferPopUp);
        }

        public void Init(UnityAction onPurchaseButtonClicked)
        {
            onPurchaseButtonSelected = onPurchaseButtonClicked;
            inited = true;
        }
        
        public void ActiveOfferButton(DynamicSpecialOfferPackage currentOffer)
        {
            if (!inited)
            {
                DebugPro.LogError<DynamicSpecialOfferLogTag>("Dynamic Special Offer Hasn't been initialized before!");
                return;
            }
            
            offer = currentOffer;
            offAmountText.SetText(offer.Tag());
            offerButtonObject.SetActive(true);
        }

        public void DeactivateOfferButton()
        {
            offerButtonObject.SetActive(false);
        }

        public void UpdateTimer(string time)
        {
            if (!inited)
            {
                DebugPro.LogError<DynamicSpecialOfferLogTag>("Dynamic Special Offer Hasn't been initialized before!");
                return;
            }
            
            offerTimerText.SetText(time);
        }

        public void UpdateStorePackagesAndOpenSpecialOfferPopUp()
        {
            if (!inited)
            {
                DebugPro.LogError<DynamicSpecialOfferLogTag>("Dynamic Special Offer Hasn't been initialized before!");
                return;
            }

            var waitPopup = Base.gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
            Base.gameManager.shopCreator.ValidateShopOpening(
                onSuccess: () =>
                {
                    waitPopup.Close();
                    OpenSpecialOfferPopup( delegate { }, delegate { });
                },
                onFailure: (result) =>
                {
                    waitPopup.Close();
                    if(result != FailureResult.PurchaseDisabled)
                        Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                            ScriptLocalization.Message_Shop.InternetRequirment,
                            ScriptLocalization.UI_General.Ok,
                            null,
                            true,
                            null);
                });
        }

        public void OpenSpecialOfferPopup(Action onPurchaseClicked, Action onClose)
        {
            if (!inited)
            {
                DebugPro.LogError<DynamicSpecialOfferLogTag>("Dynamic Special Offer Hasn't been initialized before!");
                return;
            }

            if (Base.gameManager.shopCreator.IsValidToOpen(offer))
            {
                //TODO: feature art index must be set 
                openedDynamicSpecialOfferPopup = Base.gameManager.OpenPopup<Popup_DynamicSpecialOffer>().Setup(offer, GetOfferPopupFeatureArtIndex(),
                    () =>
                    {
                        onPurchaseClicked();
                        onPurchaseButtonSelected();
                    }, onClose);
            }
        }

        private int GetOfferPopupFeatureArtIndex()
        {
            var offerIndex = Base.gameManager.profiler.PreviousDynamicSpecialOfferIndex;
            var offersCount = Base.gameManager.dynamicSpecialOfferManager.RepeatingOffers.Length;
            if (offerIndex < 0)
                return 0;
            else
            {
                var offersThird = offersCount / 3;
                if (offerIndex < offersThird)
                    return 0;
                else if (offerIndex >= offersThird && offerIndex < offersThird * 2)
                    return 1;
                else
                    return 2;
            }
        }

        public void ClosePopup()
        {
            if (openedDynamicSpecialOfferPopup)
                openedDynamicSpecialOfferPopup.Close();
        }
    }

}