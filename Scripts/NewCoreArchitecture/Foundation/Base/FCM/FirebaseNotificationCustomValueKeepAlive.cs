using UnityEngine;


public class FirebaseNotificationCustomValueKeepAlive
{
    
    private const string FIREBASE_NOTIFICATION_REWARDS_KEY = "FirebaseNotificationRewards";
    
    
    public static FirebaseGiftCollection GetNotificationGifts()
    {
        var rewardsJson = PlayerPrefs.GetString(FIREBASE_NOTIFICATION_REWARDS_KEY, "");
        if (!string.IsNullOrEmpty(rewardsJson))
            return JsonUtility.FromJson<FirebaseGiftCollection>(rewardsJson);

        return null;
    }

    public static void SaveNotificationRewards(FirebaseGiftCollection giftCollection)
    {
        var rewardsJson = JsonUtility.ToJson(giftCollection);
        PlayerPrefs.SetString(FIREBASE_NOTIFICATION_REWARDS_KEY, rewardsJson);
    }

    public static void ClearNotificationRewards()
    {
        PlayerPrefs.SetString(FIREBASE_NOTIFICATION_REWARDS_KEY, "");
    }
    
}
