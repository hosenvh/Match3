using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.GardenMechanic;
using Match3.Presentation.Gameplay.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KitchenParadise.Presentation;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{

    public class GardenGenerationPresentationHandlerImp : MonoBehaviour, GardenGenerationPresentationHandler
    {
        public GameObject tilePlaceHolderPrefab;
        public Transform effectTarget;

        public CurvedMovementInfo movementInfo;

        public void PlaceGeneratedTiles(CellStack origin, List<CellStack> targets, Action<int> onTargetReached, Action onCompleted)
        {
            var orginPresenter = origin.GetComponent<CellStackPresenter>();

            var counter = new Counter(targets.Count, onCompleted);

            for (int i = 0; i < targets.Count; ++i)
            {
               
                var tilePlaceHolder = Instantiate(tilePlaceHolderPrefab, effectTarget, false);
                tilePlaceHolder.transform.position = orginPresenter.transform.position;

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
    }
}