using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using SeganX;
using UnityEngine;
using UnityEngine.Events;


public class PurchaseBalloonController : MonoBehaviour
{

    public GameObject balloonObject;
    public LocalText itemCostText;

    private MapItem_UserSelect_Single selectedUserSelect;
    private int selectedItemIndex;
    private UnityAction<bool> onPurchase;
    
    
    public void Open(float xPos, MapItem_UserSelect_Single userSelect, int itemIndex, UnityAction<bool> purchaseCallBack)
    {
        selectedUserSelect = userSelect;
        selectedItemIndex = itemIndex;
        onPurchase = purchaseCallBack;
        
        balloonObject.SetActive(true);
        
        var balloonRectTransform = balloonObject.GetRectTransform();
        balloonRectTransform.position = new Vector3(xPos, balloonRectTransform.position.y, 0);
        var balloonLocalPosition = balloonRectTransform.localPosition;
        balloonRectTransform.localPosition = new Vector3(balloonLocalPosition.x, balloonLocalPosition.y, 0);
        
        itemCostText.SetText(selectedUserSelect.costs[selectedItemIndex].ToString());
    }

    
    public void Close()
    {
        balloonObject.SetActive(false);
    }


    public void Purchase()
    {
        var itemPurchaseService = ServiceLocator.Find<MapItemPurchaseManager>();
        var result = itemPurchaseService.TryPurchaseItem(selectedUserSelect, selectedItemIndex);
        onPurchase(result);
        
        Base.gameManager.fxPresenter.PlayClickAudio();
    }
    
}
