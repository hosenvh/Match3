using System;
using System.Collections.Generic;
using DG.Tweening;
using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.IceMakerMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;


namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class IceMakerPresentationHandlerImp : MonoBehaviour, IceMakerPresentationHandler
    {
        public IceMakerElementPresenter iceElementPrefab;
        public Transform effectTarget;

        public CurvedMovementInfo movementInfo;

        public void StartPoping(IceMakerMainTile mainTile, List<CellStack> targets, Action<int> onTargetReached, Action onCompleted)
        {
            mainTile.GetComponent<IceMakerTilePresenter>().StartPoping();

            var tilePresenter = mainTile.GetComponent<IceMakerTilePresenter>();
            var origins = tilePresenter.PopRandomIceCubes(targets.Count);

            var counter = new Counter(targets.Count, onCompleted);

            for (int i = 0; i < targets.Count; ++i)
            {
                var popedIce = Instantiate(iceElementPrefab, effectTarget, false);
                popedIce.transform.position = origins[i].position;
                popedIce.PlayPopingEffect();

                var indexCapture = i;
                popedIce.transform.DoMoveWithForce(
                          movementInfo.force,
                          movementInfo.forceAngle,
                          targets[i].GetComponent<CellStackPresenter>().transform.position,
                          movementInfo.speed,
                          false).SetEase(movementInfo.curve).SetSpeedBased(true).OnComplete(() =>
                                        {
                                            popedIce.PlayDestroyEffect(() =>
                                            {
                                                onTargetReached(indexCapture);
                                                counter.Decrement();
                                            });
                                        });
            }
        }

        public void StartRemoving(IceMakerMainTile mainTile, Action onRemoveCompleted)
        {
            mainTile.GetComponent<IceMakerTilePresenter>().StartRemoving(onRemoveCompleted);
        }
    }
}
