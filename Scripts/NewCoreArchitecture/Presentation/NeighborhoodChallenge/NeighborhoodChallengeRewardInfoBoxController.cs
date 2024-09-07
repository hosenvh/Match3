using SeganX;
using UnityEngine;


public class NeighborhoodChallengeRewardInfoBoxController : MonoBehaviour
{

    [SerializeField] private LocalText titleText = default;
    [SerializeField] private LocalText titleText2 = default;
    [SerializeField] private LocalText titleText3 = default;

    
    
    public void SetTitle(string title1, string title2, string title3)
    {
        titleText.SetText(title1);
        titleText2.SetText(title2);
        titleText3.SetText(title3);
    }

}
