using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KitchenParadise.Presentation;
using Match3.Foundation.Base.ServiceLocating;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class VacuumCleanerTilePresenter : TilePresenter
    {
        [Serializable]
        public class VacuumCleanerDirectionalPresentationInfo
        {
            public Direction direction;
            public GameObject root;
            public SkeletonGraphic skeletonGraphic;
            public GameObject tank;
            public Image fillImage1;
            public Image fillImage2;
        }

        public List<VacuumCleanerDirectionalPresentationInfo> directionalInfos;

        public Color mainFillColor;
        public float secondFillOffset;

        public SkeletonDataAsset upSpineSkeletonDataAsset;
        public SkeletonDataAsset downSpineSkeletonDataAsset;
        public SkeletonDataAsset sideSpineSkeletonDataAsset;


        public AudioClip startAudioClip;
        public AudioClip endAudioClip;

        [SpineAnimation(dataField: "upSpineSkeletonDataAsset")]
        public string idleAnimation;

        [SpineAnimation(dataField: "upSpineSkeletonDataAsset")]
        public string startAnimation;

        [SpineAnimation(dataField: "upSpineSkeletonDataAsset")]
        public string movingAnimation;

        VacuumCleaner vacuumCleaner;

        VacuumCleanerDirectionalPresentationInfo currentDirectionInfo;

        protected override void InternalSetup()
        {
            vacuumCleaner = tile as VacuumCleaner;

            vacuumCleaner.onFillChanged += UpdateFill;

            var directionInfo = directionalInfos.Find(i => i.direction == vacuumCleaner.direction);
            ChangePresentationInfoTo(directionInfo);

            UpdateFill(0);
            currentDirectionInfo.skeletonGraphic.AnimationState.SetAnimation(0, idleAnimation, false);
        }

        private void ChangePresentationInfoTo(VacuumCleanerDirectionalPresentationInfo directionInfo)
        {
            currentDirectionInfo = directionInfo;

            currentDirectionInfo.root.SetActive(true);
            currentDirectionInfo.fillImage2.color = mainFillColor;
        }

        public void PlayStartEffect(Action onCompleted)
        {
            ServiceLocator.Find<GameplaySoundManager>().TryPlay(startAudioClip);
            currentDirectionInfo.skeletonGraphic.AnimationState.SetAnimation(0, startAnimation, onCompleted);
        }

        public void PlayMovingEffect()
        {
            currentDirectionInfo.skeletonGraphic.AnimationState.SetAnimation(0, movingAnimation, true);
        }

        public void PlayEndEffect()
        {
            ServiceLocator.Find<GameplaySoundManager>().TryStop(startAudioClip);
            ServiceLocator.Find<GameplaySoundManager>().TryPlay(endAudioClip);
        }

        private void OnDestroy()
        {

            vacuumCleaner.onFillChanged -= UpdateFill;
        }

        private void UpdateFill(int fill)
        {
            var fillRate = fill / (float)vacuumCleaner.targetNumber;
            currentDirectionInfo.fillImage1.fillAmount = fillRate;
            currentDirectionInfo.fillImage2.fillAmount = fillRate + secondFillOffset;

            if (vacuumCleaner.IsFilled())
                currentDirectionInfo.tank.SetActive(false);
        }

        public Transform TankTransform()
        {
            return currentDirectionInfo.tank.transform;
        }
    }
}