using System;
using System.Collections.Generic;
using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Match3.Presentation.Gameplay.Tiles

{
    public class BeachBallTilePresenter : TilePresenter
    {
        [Serializable]
        public struct ColorRemovalActionEntry
        {
            public TileColor color;
            public BeachBallColorPresenter presenter;
            public UnityEvent action;
        }

        public GameObject colorsContainer;



        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string idleAnimation;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string destroyAnimation;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string activationAnimation;

        public UnityEvent onActivationStarted;
        public TileUnityEvent onPopingStarted;

        BeachBallMainTile beachBallMainTile;

        List<BeachBallColorPresenter> beachBallPresenters = new List<BeachBallColorPresenter>();

        protected override void InternalSetup()
        {
            beachBallMainTile = tile as BeachBallMainTile;
            beachBallMainTile.onColorRemoved += OnColorRemoved;

            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimation, false);

            beachBallPresenters.AddRange(colorsContainer.GetComponentsInChildren<BeachBallColorPresenter>());
        }

        private void OnColorRemoved(TileColor color)
        {
            beachBallPresenters.Find(p => p.color == color).PlayDisappearingEffect();
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            if (beachBallMainTile.IsDestroyed())
            {
                skeletonGraphic.AnimationState.SetAnimation(0, destroyAnimation, onCompleted);
                onPopingStarted.Invoke(this);
            }
        }


        public void PlayActivationEffect(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, activationAnimation, onCompleted);
            onActivationStarted.Invoke();
        }

        private void OnDestroy()
        {
            beachBallMainTile.onColorRemoved -= OnColorRemoved;
        }

    }
}