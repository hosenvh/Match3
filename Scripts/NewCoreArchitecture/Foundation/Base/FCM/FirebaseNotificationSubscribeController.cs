using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseNotificationSubscribeController
{

    private string PreviousSubscribedLanguage
    {
        get => PlayerPrefs.GetString("NotificationSubscribedLanguage", "");
        set => PlayerPrefs.SetString("NotificationSubscribedLanguage", value);
    }
    
    public void SubscribeForLanguage(string language)
    {
        var prev = PreviousSubscribedLanguage;
        if (prev == language) return;
        PreviousSubscribedLanguage = language;
        if(!string.IsNullOrEmpty(prev))
            Firebase.Messaging.FirebaseMessaging.UnsubscribeAsync($"/topics/{prev}");
        Firebase.Messaging.FirebaseMessaging.SubscribeAsync($"/topics/{language}");
    }

}
