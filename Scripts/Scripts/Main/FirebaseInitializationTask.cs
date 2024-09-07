using System;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.Extensions;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using UnityEngine;

namespace Match3.Main
{
    public class FirebaseInitializationTask : BasicTask
    {
        protected override void InternalExecute(Action onComplete, Action onAbort)
        {
            if (!FirebaseFunctionalities.IsDeviceCompatibleWithFirebase())
            {
                onComplete();
                return;
            }
            
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    var marketName = ServiceLocator.Find<IMarketManager>().GetMarketName();
                    Firebase.Messaging.FirebaseMessaging.SubscribeAsync($"/topics/{marketName}");
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                    Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                    Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

                    var userId = ServiceLocator.Find<UserProfileManager>().GlobalUserId;
                    Crashlytics.SetUserId(userId);
                    

                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                    // When this property is set to true, Crashlytics will report all
                    // uncaught exceptions as fatal events. This is the recommended behavior.
                    Crashlytics.IsCrashlyticsCollectionEnabled = true;


                    FirebaseFunctionalities.IsFirebaseInitialized = true;
                }
                else 
                    Debug.LogError(task.Result);
                onComplete();
            });
        }

        static void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            //        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        }

        static void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            //        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
        }

        // public float Progress()
        // {
            // return 0;
        // }
        
        
        
        
    }
}