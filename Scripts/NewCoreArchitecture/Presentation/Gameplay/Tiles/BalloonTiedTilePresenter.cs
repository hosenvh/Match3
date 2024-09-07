using KitchenParadise.Presentation;
using Spine.Unity;
using System;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;



namespace Match3.Presentation.Gameplay.Tiles
{
    public class BalloonTiedTilePresenter : SpineTilePresenter
    {
        [SerializeField] private SkeletonGraphic ropeSkeletonGraphic;

        protected override void OnPreSetup()
        {
        }

        public void SetRopeParent(Transform parent)
        {
            if (ropeSkeletonGraphic)
                ropeSkeletonGraphic.transform.SetParent(parent);
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            base.PlayHitAnimation(onCompleted);
            if (ropeSkeletonGraphic)
                ropeSkeletonGraphic.AnimationState.SetAnimation(0, popAnimations[tile.CurrentLevel()], delegate { });
        }

        private void OnDestroy()
        {
            if (ropeSkeletonGraphic)
                Destroy(ropeSkeletonGraphic.gameObject);
        }
    }
}