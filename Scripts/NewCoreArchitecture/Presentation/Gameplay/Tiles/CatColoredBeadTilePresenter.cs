using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Match3.Presentation.Gameplay.Tiles
{
    // NOTE: This is a placeholder implementation.
    public class CatColoredBeadTilePresenter : TilePresenter
    {
        protected override void InternalSetup()
        {

        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            this.transform.DOScale(0, 0.3f).OnComplete(() => onCompleted.Invoke());
        }
    }
}