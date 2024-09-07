using DynamicSpecialOfferSpace;
using UnityEngine;




public class ClickablesInputsController : MonoBehaviour
{
    
    private const string ClickableLayerName = "Clickable";
    public static int ClickableDetectionLayer 
    {
        get
        {
            if(clickableDetectionLayer == -1)
                clickableDetectionLayer = LayerMask.NameToLayer(ClickableLayerName);
            return clickableDetectionLayer;
        }
    }

    private static int clickableDetectionLayer = -1;

    // ------------------------------------------ Serialized Or Public Fields ------------------------------------------ \\

    [SerializeField] private Camera cam = default;
    [SerializeField] private bool workWhilePopupIsOpen = false;

    public int maxDis = 100;
    
    // ------------------------------------------ Private Fields ------------------------------------------ \\
    
    private IClickable lastClickable;
    
    Vector3 clickPos = Vector3.zero;
    
    private bool pressedDown    = false;
    private bool pressedUp      = false;
    private bool correctClicked = false;

    // ==================================================================================================== \\
    
    
    // private void Awake()
    // {
    //     ClickableDetectionLayer = 1 << LayerMask.NameToLayer(ClickableLayerName);
    // }

    private void Update()
    {

        if (!workWhilePopupIsOpen && IsAnyPopupOpenExceptMainMenu() || IsDynamicSpecialOfferOpen()) 
            return;

#if UNITY_EDITOR

        pressedDown  = Input.GetMouseButtonDown(0);
        pressedUp    = Input.GetMouseButtonUp(0);
        
        if (!pressedDown && !pressedUp) return;
        
        clickPos = Input.mousePosition;
        
#elif (UNITY_ANDROID || UNITY_IOS)

        if (Input.touchCount == 0) return;
        pressedDown   = Input.touches[0].phase == TouchPhase.Began;
        pressedUp     = Input.touches[0].phase == TouchPhase.Ended;

        if (!pressedDown && !pressedUp) return;

        clickPos = Input.touches[0].position;
        
#endif

        if (pressedDown)
        {
            var ray = cam.ScreenPointToRay(clickPos);
            if (!Physics.Raycast(ray, out var hit, maxDis, 1 << ClickableDetectionLayer)) return;
            lastClickable = hit.transform.GetComponent<IClickable>();
            correctClicked = true;
            lastClickable?.ClickDown();
        }
        else if (pressedUp && correctClicked)
        {
            lastClickable?.ClickUp();
            correctClicked = false;
        }
        
    }

    private bool IsAnyPopupOpenExceptMainMenu()
    {
        return !(Base.gameManager.CurrentPopup is Popup_MainMenu);
    }

    private bool IsDynamicSpecialOfferOpen()
    {
        return DynamicSpecialOfferPopUp.IsOpen;
    }
}
