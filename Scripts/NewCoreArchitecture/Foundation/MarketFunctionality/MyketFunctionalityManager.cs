using UnityEngine;

namespace Match3.Foundation.Base
{
    public class MyketFunctionalityManager : BasicStoreFunctionalityManager
    {
        public MyketFunctionalityManager(string developerId) : base(developerId)
        {
        }

        public override long LastAvailableApplicationVersion()
        {
            throw new System.NotImplementedException();
        }

        protected override void InternalRequestRatingFor(string url)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
        intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "myket://comment?id="+url));
        intentObject.Call<AndroidJavaObject>("setPackage", "ir.mservices.market");

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
        intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "myket://details?id="+url));
        intentObject.Call<AndroidJavaObject>("setPackage", "ir.mservices.market");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);
#endif
        }


        protected override void InternalRequestVisitDeveloperPage(string devId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR 
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
        intentObject.Call<AndroidJavaObject>("setData", 
                uriClass.CallStatic<AndroidJavaObject>("parse", "myket://developer/" + devId));
        intentObject.Call<AndroidJavaObject>("setPackage", "ir.mservices.market");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);
#endif
        }

        public override string StoreName()
        {
            return "Myket";
        }

        protected override string StoreBaseURL()
        {
            return "https://myket.ir/app/";
        }
    }
}