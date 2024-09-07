using SeganX;
using UnityEngine;
using UnityEngine.UI;


public class NeighborhoodChallengeRewardPresentationController : MonoBehaviour
{
    
    // -------------------------------------------- Public  / Serialized Fields -------------------------------------------- \\ 
    
    [SerializeField] private Image rewardIconImage = default;
    [SerializeField] private LocalText rewardCountText = default;
    [SerializeField] private GameObject xObject = default;
    [SerializeField] private RectTransform layoutRectTransform = default;
    
    // ===================================================================================================================== \\
    
    
    public void Setup(Sprite icon, string rewardCount, bool haveX)
    {
        rewardIconImage.sprite = icon;
        rewardCountText.SetText(rewardCount);
        xObject.SetActive(haveX);
        
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRectTransform);
    }
    
    
}
