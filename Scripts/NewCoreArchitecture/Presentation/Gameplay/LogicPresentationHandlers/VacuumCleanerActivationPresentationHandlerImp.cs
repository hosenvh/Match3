using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.VacuumCleanerMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;

using DG.Tweening;
using Match3.Presentation.Gameplay.Core;
using System;
using Match3.Foundation.Base.ServiceLocating;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{

    public class VacuumCleanerActivationPresentationHandlerImp : MonoBehaviour, VacuumCleanerActivationPresentationHandler
    {
        public float moveSpeed;
        public Transform targetContainer;


        public void Activate(VacuumCleaner vacuumCleaner, List<CellStack> cellStackList, Action<CellStack> onCellStackReached, Action onCompleted)
        {
            var cleanerPresenter = vacuumCleaner.GetComponent<VacuumCleanerTilePresenter>();

            cleanerPresenter.Owner().transform.SetParent(targetContainer);
            cleanerPresenter.PlayStartEffect(() => StartMoving(cleanerPresenter, cellStackList, onCellStackReached, onCompleted));

        }

        private void StartMoving(VacuumCleanerTilePresenter cleanerPresenter, List<CellStack> cellStackList, Action<CellStack> onCellStackReached, Action onCompleted)
        {
            cleanerPresenter.PlayMovingEffect();

            MoveTo(0, cleanerPresenter, cellStackList, onCellStackReached, onCompleted);
        }

        private void MoveTo(int index, VacuumCleanerTilePresenter cleanerPresenter, List<CellStack> cellStackList, Action<CellStack> onCellStackReached, Action onCompleted)
        {
            var targetCellStack = cellStackList[index];
            var targetPresenter = targetCellStack.GetComponent<CellStackPresenter>();

            var tween = cleanerPresenter.transform.
                DOMove(targetPresenter.transform.position, moveSpeed).
                SetSpeedBased(true).
                SetEase(Ease.Linear).
                OnComplete(() => HandleCellStackReaching(index, cleanerPresenter, cellStackList, onCellStackReached, onCompleted));

        }

        void HandleCellStackReaching(int index, VacuumCleanerTilePresenter cleanerPresenter, List<CellStack> cellStackList, Action<CellStack> onCellStackReached, Action onCompleted)
        {
            onCellStackReached(cellStackList[index]);
            if (index == cellStackList.Count - 1)
            {
                cleanerPresenter.PlayEndEffect();
                onCompleted();
            }
            else
                MoveTo(index + 1, cleanerPresenter, cellStackList, onCellStackReached, onCompleted);
        }
    }
}