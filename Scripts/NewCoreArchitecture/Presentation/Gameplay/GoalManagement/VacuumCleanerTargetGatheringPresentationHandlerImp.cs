using System;
using DG.Tweening;
using KitchenParadise.Presentation;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;

namespace Match3.Presentation.Gameplay.GoalManagement
{
    public class VacuumCleanerTargetGatheringPresentationHandlerImp : MonoBehaviour, VacuumCleanerTargetGatheringPresentationHandler
    {
        public CurvedMovementInfo movementInfo;
        public float scaleSpeed;
        public float scaleAmount;

        public Transform effectContainer;

        public void Gather(Tile tile, VacuumCleaner vacuumCleaner, Action<Tile> onDetached, Action onGathered)
        {

            var presenter = tile.GetComponent<TilePresenter>();
            var target = vacuumCleaner.GetComponent<VacuumCleanerTilePresenter>().TankTransform();

            presenter.transform.SetParent(effectContainer);

            presenter.transform.DOScale(scaleAmount, scaleSpeed).SetSpeedBased(true);

            presenter.transform.
                DoMoveWithForce(movementInfo.force, movementInfo.forceAngle, target.position, movementInfo.speed, false).
                SetSpeedBased(true).
                SetEase(movementInfo.curve).
                OnComplete(
                () => { onGathered(); presenter.Destroy(); });

            ServiceLocator.Find<UnityTimeScheduler>().Schedule(0.2f, () => onDetached(tile), this);
        }
    }

}