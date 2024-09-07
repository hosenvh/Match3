using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KitchenParadise.Presentation;
using Match3.Game.Gameplay.SubSystems.ChickenNestMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Tiles;


namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class ChickenNestPresentationHandlerImp : MonoBehaviour, ChickenNestPresentationHandler
    {
        public GameObject tilePlaceHolderPrefab;
        public Transform effectTarget;

        public CurvedMovementInfo movementInfo;

        public void PlaceGeneratedTilesIn(CellStack origin, List<CellStack> targets, Action<int> onTargetReached, Action onCompleted)
        {
            var originPresenter = origin.GetComponent<CellStackPresenter>();

            var counter = new Counter(targets.Count, onCompleted);

            for (int i = 0; i < targets.Count; ++i)
            {
                var tilePlaceHolder = Instantiate(tilePlaceHolderPrefab, effectTarget, false);
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
            }
        }

        public void StartRemoving(ChickenNest mainTile)
        {
            mainTile.GetComponent<ChickenNestTilePresenter>().PlayDestroyingAnimation();
        }
    }
}