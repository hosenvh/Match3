using System;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using RTLTMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay.Tiles
{

    public class GasCylinderTilePresenter : SpineTilePresenter
    {
        public RTLTextMeshPro text;
        public int criticalLevelThreshold;
        public UnityEvent onReachedCriticalLevel;

        protected override void OnPreSetup()
        {
            var gasCylinder = tile.As<GasCylinder>();
            gasCylinder.onCountdownChanged += UpdateCountdown;
            UpdateCountdown(gasCylinder.CurrentCountdown());
        }

        private void OnDestroy()
        {
            tile.As<GasCylinder>().onCountdownChanged -= UpdateCountdown;
        }

        private void UpdateCountdown(int countdown)
        {
            if (countdown == criticalLevelThreshold)
                onReachedCriticalLevel.Invoke();
            text.text = countdown.ToString();
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            if (tile.CurrentLevel() == 0)
                text.gameObject.SetActive(false);
            base.PlayHitAnimation(onCompleted);
        }
    }
}