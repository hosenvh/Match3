using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using System;
using System.Collections.Generic;
using KitchenParadise.Presentation;
using Spine;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class FireflyJarPresenter : TilePresenter
    {
        public SkeletonGraphic skeletonGraphic;

        [SpineBone(dataField: "skeletonDataAsset")]
        public string fireflyExitBone;

        [SpineEvent(dataField: "skeletonDataAsset")]
        public string fireflyExitEvent;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> idleAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> popAnimations;

        event Action onFireflyExistedEvent = delegate{};


        protected sealed override void InternalSetup()
        {
            skeletonGraphic.AnimationState.SetAnimation(
                0, 
                idleAnimations[UnityEngine.Mathf.Min(tile.CurrentLevel() - 1, idleAnimations.Count-1)],
                false);

            skeletonGraphic.AnimationState.Event += HandleEvent;
        }



        protected override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.AddAnimation(
                0,
                popAnimations[UnityEngine.Mathf.Min(tile.CurrentLevel(), popAnimations.Count - 1)],
                onCompleted);
        }

        public void RegisterOnFireflyExited(Action onFireflyExited)
        {
            onFireflyExistedEvent += onFireflyExited;
        }

        public void UnRegisterOnFireflyExited(Action onFireflyExited)
        {

            onFireflyExistedEvent -= onFireflyExited;
        }


        private void HandleEvent(TrackEntry track, Event evt)
        {
            if (evt.data.name.Equals(fireflyExitEvent))
                onFireflyExistedEvent();
        }

        public UnityEngine.Vector3 FireFlyExitPosition()
        {
            var bone = skeletonGraphic.Skeleton.FindBone(fireflyExitBone);
            bone.UpdateWorldTransform();
            // TODO: the 100f must be read from the skeleton data.
            return bone.GetWorldPosition(skeletonGraphic.transform, 100f);
        }
    }
}