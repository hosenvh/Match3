using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseFunctionalities
{

    public static bool IsFirebaseInitialized = false;
    
    public static bool IsDeviceCompatibleWithFirebase()
    {
        try
        {
            return IsPlayServicesAvailable();
        }
        catch(Exception e)
        {
            Debug.LogError($"Couldn't detect PlayerService : {e}");
            return false;
        }
    }

    private static bool IsPlayServicesAvailable()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_ANDROID
        const string GoogleApiAvailability_Classname =
            "com.google.android.gms.common.GoogleApiAvailability";
        AndroidJavaClass clazz =
            new AndroidJavaClass(GoogleApiAvailability_Classname);
        AndroidJavaObject obj =
            clazz.CallStatic<AndroidJavaObject>("getInstance");

        var androidJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = androidJC.GetStatic<AndroidJavaObject>("currentActivity");

        int value = obj.Call<int>("isGooglePlayServicesAvailable", activity);

        // 0 == success
        // 1 == service_missing
        // 2 == update service required
        // 3 == service disabled
        // 18 == service updating
        // 9 == service invalid
        return value == 0;
#endif
    }
}
