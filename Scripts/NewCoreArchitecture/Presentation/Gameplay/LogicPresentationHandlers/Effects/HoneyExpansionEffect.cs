#pragma warning disable CS0109
using System;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using KitchenParadise.Presentation;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class HoneyExpansionEffect : MonoBehaviour
    {
        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: nameof(skeletonGraphic))]
        public new string animation;



        public void Play(Action onCompleted)
        {

            skeletonGraphic.AnimationState.SetAnimation(0, animation, () => { onCompleted(); Destroy(this.gameObject); });
        }
    }
}