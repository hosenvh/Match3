using SeganX;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LeaderboardScoreBoxPresentationController : MonoBehaviour
{

    // -------------------------------------------- Public  / Serialized Fields -------------------------------------------- \\ 
    
    [SerializeField] private Image underRankImage = default;
    [SerializeField] private LocalText rankText = default;
    [SerializeField] private LocalText usernameText = default;
    [SerializeField] private LocalText scoreText = default;
    
    [Space(10)]
    [SerializeField] private Image giftBoxImage = default;
    [SerializeField] private Button giftBoxButton = default;
    [SerializeField] private Image specialRewardIcon = default;

    // -------------------------------------------- Private Fields -------------------------------------------- \\ 
    
    
    private int myRank;
    
    
    // ===================================================================================================================== \\
    
    
    public LeaderboardScoreBoxPresentationController Fill(Sprite rankIcon, Sprite selectedSpecialRewardIcon, Sprite giftBoxSprite, int rank, string userName, int score)
    {
        myRank = rank;
        
        underRankImage.sprite = rankIcon;
        usernameText.SetText(userName);
        rankText.SetText(rank.ToString());
        scoreText.SetText(score.ToString());
        giftBoxImage.sprite = giftBoxSprite;
        specialRewardIcon.sprite = selectedSpecialRewardIcon;
        specialRewardIcon.gameObject.SetActive(selectedSpecialRewardIcon != null);

        return this;
    }


    public LeaderboardScoreBoxPresentationController SetOnGiftBoxClickAction(UnityAction<int, Vector3> onClick)
    {
        giftBoxButton.onClick.AddListener(() =>
        {
            onClick(myRank, giftBoxButton.transform.position);
        });
        return this;
    }
    
    
    public void SetUsername(string userName)
    {
        usernameText.SetText(userName);
    }
    
}
