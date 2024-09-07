

using Match3.Foundation.Base;
using UnityEngine;

namespace Match3.Foundation.Unity
{
    public class GooglePlayFunctionalityManager : BasicStoreFunctionalityManager
    {

        public GooglePlayFunctionalityManager(string developerId) : base(developerId)
        {
        }
        
        
        public override long LastAvailableApplicationVersion()
        {
            throw new System.NotImplementedException();
        }

        protected override void InternalRequestRatingFor(string url)
        {
            // TODO: What's the difference between the two approaches?


            //UnityEngine.Application.OpenURL(
            //    "https://play.google.com/store/apps/details?id=" + url);

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
                intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "https://play.google.com/store/apps/details?id=" + url));

    intentObject.Call<AndroidJavaObject>("setPackage", "com.android.vending");

    AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intentObject);
#endif
        }

        protected override void InternalRequestVisitPageFor(string url)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
                intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "https://play.google.com/store/apps/details?id=" + url));

    intentObject.Call<AndroidJavaObject>("setPackage", "com.android.vending");

    AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intentObject);
#endif

            //UnityEngine.Application.OpenURL(
            //    "https://play.google.com/store/apps/details?id=" + url);
        }

        
        protected override void InternalRequestVisitDeveloperPage(string devId)
        {
            Application.OpenURL($"https://play.google.com/store/apps/dev?id={devId}");
        }
        
        
        public override string StoreName()
        {
            return "GooglePlay";
        }

        protected override string StoreBaseURL()
        {
            return "https://play.google.com/store/apps/details?id=";
        }
    }
}