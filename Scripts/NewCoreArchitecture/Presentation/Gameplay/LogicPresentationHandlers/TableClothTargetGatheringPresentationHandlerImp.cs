using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;

using DG.Tweening;
using Match3.Presentation.Gameplay.Core;
using System;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Matching;


namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class TableClothTargetGatheringPresentationHandlerImp : MonoBehaviour, TableClothTargetGatheringPresentationHandler
    {
        public float force;
        public float forceAngle;
        public float scaleSpeed;
        public float scaleAmount;
        public float movementSpeed;
        public Transform effectContainer;

        public void Gather(Tile tile, TableClothMainTile tableClothMain, TableClothMainTile.TargetHandler target, Action<Tile> onDetached, Action onGathered)
        {

            var presenter = tile.GetComponent<TilePresenter>();
            var presentationTarget = tableClothMain.GetComponent<TableClothTilePresenter>().GoalTransform(tile);

            presenter.transform.SetParent(effectContainer);

            presenter.transform.DOScale(scaleAmount, scaleSpeed).SetSpeedBased(true);

            presenter.transform.
                DoMoveWithForce(force, forceAngle, presentationTarget.transform.position, movementSpeed, false).
                SetSpeedBased(true).
                SetEase(Ease.Linear).
                OnComplete(
                () => { onGathered(); presenter.Destroy(); });

            ServiceLocator.Find<UnityTimeScheduler>().Schedule(0.2f, () => onDetached(tile), this);
        }
    }
}
