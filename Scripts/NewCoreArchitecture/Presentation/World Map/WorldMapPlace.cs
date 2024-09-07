using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity;
using Match3.Foundation.Unity.TimeManagement;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.WorldMap
{
    public class WorldMapPlace : MonoBehaviour
    {
        [MapSelector]
        [SerializeField] private string placeMapId;
        [SerializeField] private Button button;

        [Space(10)] 
        [SerializeField] private Animation animationPlayer;

        public string PlaceMapId => placeMapId;

        private bool canClick = true;
        
        
        public void SetOnButtonClickListener(Action<WorldMapPlace> onClickAction)
        {
            button.onClick.AddListener(() =>
            {
                if(canClick)
                    onClickAction(this);
            });
        }

        public void PlayAnimation(AnimationClip clip, Action onAnimationFinished)
        {
            animationPlayer.clip = clip;
            animationPlayer.Play();
            canClick = false;
            ScheduleAnimationFinishedCallBack(clip, onAnimationFinished);
        }

        private void ScheduleAnimationFinishedCallBack(AnimationClip clip, Action onFinished)
        {
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(clip.length, () =>
            {
                onFinished();
                canClick = true;
            }, this);
        }
        
    }    
}


