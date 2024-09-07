using UnityEngine;
using System;
using System.Collections.Generic;
using KitchenParadise.Presentation;
using DG.Tweening;
using Match3.Game.Gameplay.SubSystems.DuckMechanic;
using Match3.Presentation.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class DuckPresentationHandlerImp : MonoBehaviour, DuckDestructionPresentationHandler, DuckMovementPresentationHandler
    {
        public Transform effectTarget;
        public CurvedMovementInfo movementInfo;
        public Vector3 scaleAmount;
        public float scaleSpeed;

        public void Move (List<MovementData> movementsData, Action<MovementData> onCompleted)
        {
            foreach (var data in movementsData)
            {
                var presenter = data.duck.GetComponent<DuckTilePresenter>();
                presenter.PlayDisapperAnimation(() => Appear(presenter, data, onCompleted));
            }
        }

        private void Appear (DuckTilePresenter presenter, MovementData data, Action<MovementData> onCompleted)
        {
            presenter.Owner().SetLogicPositionUpdateFlag(false);
            presenter.Owner().transform.position = data.target.Parent().GetComponent<CellStackPresenter>().transform.position;

            onCompleted(data);
            presenter.PlayAppearAnimation(() => { presenter.Owner().SetLogicPositionUpdateFlag(true); });
        }

        public void PlaceGeneratedTilesIn (Duck duck, CellStack origin, List<CellStack> targets, Action<int> onTargetReached, Action onCompleted)
        {
            var DuckTilePresenter = duck.GetComponent<DuckTilePresenter>();
            var originPresenter = origin.GetComponent<CellStackPresenter>();
            var counter = new Counter(targets.Count, onCompleted);

            for (int i = 0; i < targets.Count; i++)
            {
                var tilePlaceHolder = DuckTilePresenter.GeCurrentItem();
                tilePlaceHolder.transform.SetParent(effectTarget);
                tilePlaceHolder.transform.position = originPresenter.transform.position;

                var indexCapture = i;
                tilePlaceHolder.transform.DoMoveWithForce(
                    movementInfo.force,
                    movementInfo.forceAngle,
                    targets[i].GetComponent<CellStackPresenter>().transform.position,
                    movementInfo.speed,
                    false).
                    SetEase(movementInfo.curve).
                    SetSpeedBased(true).
                    OnComplete(() => {
                        onTargetReached(indexCapture);
                        counter.Decrement();
                        Destroy(tilePlaceHolder.gameObject);
                });
                tilePlaceHolder.transform.DOScale(scaleAmount, scaleSpeed);
            }
        }

        public void StartRemoving (Duck mainTile)
        {
            mainTile.GetComponent<DuckTilePresenter>().PlayDestroyingAnimation();
        }
    }
}