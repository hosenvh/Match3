using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using KitchenParadise.Presentation;
using Match3.Presentation.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Spine.Unity;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class DuckTilePresenter : TilePresenter
    {
        [SerializeField] private DuckTileElementsDatabaseConfig elementsConfig = null;
        [SerializeField] private Transform itemsGrid = null;
        [SerializeField] private Image samplePrefab = null;

       [Serializable]
        public struct AnimationEntry
        {
            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string idleAnimation;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string hitAnimation;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string appearingAnimation;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string disappearingAnimation;
        }

        [Serializable]
        public struct ItemAnimationEntry
        {
            public string appearingAnimation;
            public string disappearingAnimation;
        }

        private Animation itemsGridAnimation;
        private string currentItemAnimation;
        private bool isHitted = false;
        public SkeletonGraphic[] skeletonGraphics;
        public AnimationEntry animationEntry;
        public ItemAnimationEntry itemGridAnimationEntry;

        protected override void InternalSetup ()
        {
            SetupVisual();
            itemsGridAnimation = itemsGrid.GetComponent<Animation>();

            PlayItemAnimation(itemGridAnimationEntry.appearingAnimation);
            PlayDuckAnimation(animationEntry.appearingAnimation, delegate {
                PlayIdleAnimation();
            });
        }

        public void SetupVisual ()
        {
            Duck duck = tile.As<Duck>();
            IEnumerable<DuckItem> items = duck.GetChildItems();

            foreach (DuckItem item in items)
                Instantiate(samplePrefab, itemsGrid).sprite = elementsConfig.GetVisual(item);
        }

        public GameObject GeCurrentItem ()
        {
            return itemsGrid.transform.GetChild(0).gameObject;
        }

        protected override void PlayHitAnimation (Action onCompleted)
        {
            if (tile.IsDestroyed())
            {
                onCompleted.Invoke();
                return;
            }

            PlayDuckAnimation(animationEntry.hitAnimation, onCompleted);
            isHitted = true;
        }

        public void PlayAppearAnimation (Action onCompleted)
        {
            if (tile.IsDestroyed() || isHitted)
            {
                onCompleted.Invoke();
                return;
            }

            PlayItemAnimation(itemGridAnimationEntry.appearingAnimation);
            PlayDuckAnimation(animationEntry.appearingAnimation, delegate {
                PlayIdleAnimation();
                onCompleted.Invoke();
            });
        }

        public void PlayDisapperAnimation (Action onCompleted)
        {
            PlayItemAnimation(itemGridAnimationEntry.disappearingAnimation);
            PlayDuckAnimation(animationEntry.disappearingAnimation, onCompleted);
        }

        public void PlayDestroyingAnimation ()
        {
            PlayDuckAnimation(animationEntry.disappearingAnimation, false);
        }
       
        public void PlayIdleAnimation ()
        {
            if (isHitted)
                return;
            
            PlayDuckAnimation(animationEntry.idleAnimation, true);
        }

        private void PlayDuckAnimation (string animation, bool isLoop)
        {
            foreach (SkeletonGraphic skeletonGraphic in skeletonGraphics)
                skeletonGraphic.AnimationState.SetAnimation(0, animation, isLoop);
        }

        private void PlayDuckAnimation (string animation, Action onCompleted)
        {
            foreach (SkeletonGraphic skeletonGraphic in skeletonGraphics)
            {
                skeletonGraphic.AnimationState.ClearTracks();
                skeletonGraphic.AnimationState.SetAnimation(0, animation, onCompleted);
            }
        }

        private void PlayItemAnimation (string animation)
        {
            if (animation == currentItemAnimation) // Check to avoid playing animation twice
                return;

            currentItemAnimation = animation;    
            itemsGridAnimation.Play(animation);
        }
    }
}