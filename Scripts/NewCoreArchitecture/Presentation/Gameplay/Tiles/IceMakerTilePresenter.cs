using System;
using System.Collections.Generic;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;


namespace Match3.Presentation.Gameplay.Tiles
{
    public class IceMakerTilePresenter : TilePresenter
    {
        [SerializeField] private List<Transform> iceCubes = new List<Transform>();
        [Space]
        [SerializeField] private AnimationClip popingAnimationClip;
        [SerializeField] private AnimationClip removingAnimationClip;
        [SerializeField] private AnimationPlayer animationPlayer;
        [Space]
        [SerializeField] private TileUnityEvent onPopingStarted;
        [SerializeField] private TileUnityEvent onRemovingStarted;

        protected override void InternalSetup()
        {
        }

        public void StartPoping()
        {
            onPopingStarted.Invoke(this);
            animationPlayer.Play(popingAnimationClip);
        }

        public void StartRemoving(Action onRemovingCompleted)
        {
            onRemovingStarted.Invoke(this);
            animationPlayer.Play(removingAnimationClip, onRemovingCompleted);
        }

        public List<Transform> PopRandomIceCubes(int count)
        {
            List<Transform> selectedIceCubes = new List<Transform>(count);
            for (int i = 0; i < count; i++)
            {
                Transform iceCube = iceCubes.RandomOne();
                iceCubes.Remove(iceCube);
                iceCube.gameObject.SetActive(false);
                selectedIceCubes.Add(iceCube);
            }
            return selectedIceCubes;
        }
    }
}