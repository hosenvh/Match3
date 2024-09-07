#if UNITY_EDITOR
#pragma warning disable CS0162
#endif
using System;
using Match3.Main.VideoPlayer;
using Match3.Utility;
using UnityEngine;


namespace Match3.Game.TaskManagement
{

    public class IntroVideoMapTask : MonoConditionalTask
    {
        public static IntroVideoPlayer introVideoPlayer;

        // This flag is only for backward usage, remove when it was safe
        private const string IsFirstRunString = "IsFirstRun";
        private bool IsFirstRun => PlayerPrefs.GetInt(IsFirstRunString, 1) == 1;

        private const string IsIntroVideoPlayedKey = "IsIntroVideoPlayed_TaskKey";
        private bool IsIntroVideoPlayed
        {
            get => PlayerPrefsEx.GetBoolean(IsIntroVideoPlayedKey, false);
            set => PlayerPrefsEx.SetBoolean(IsIntroVideoPlayedKey, value);
        }



        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            #if UNITY_EDITOR
            HandleIntroVideoPlayCompleted();
            return;
            #endif
            introVideoPlayer.Play(() => { }, result => { HandleIntroVideoPlayCompleted(); }, true);

            void HandleIntroVideoPlayCompleted()
            {
                introVideoPlayer = null;
                IsIntroVideoPlayed = true;
                onComplete();
            }
        }

        protected override bool IsConditionSatisfied()
        {
            // This methode is for backward compatibility and should remove when most users migrated to new version
            ConvertOldFlagData();

            return introVideoPlayer != null && !IsIntroVideoPlayed;
        }

        private void ConvertOldFlagData()
        {
            if (IsIntroVideoPlayed) return;
            IsIntroVideoPlayed = !IsFirstRun;
        }
    }

}