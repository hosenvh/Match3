using Match3.Game.Gameplay;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.CellAttachments;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.GoalManagement;
using Match3.Presentation.Gameplay.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{
    public class HitPresentationHandler : 
        MonoBehaviour, 
        TileStackHitPresentationHandler, 
        CellStackHitPresentationHandler,
        CellAttachmentHitPresentationHandler

    {

        public GoalTargetGatheringPresentationHandlerImp targetToGoalMovementPresentationController;

        public void ExecuteEffectOn(Tile tile, Action onCompleted)
        {
            var targetTile = tile;
            if (tile is SlaveTile slaveTile)
                targetTile = slaveTile.Master();

            targetTile.GetComponent<TilePresenter>().PlayHitEffect(onCompleted);
            if (targetToGoalMovementPresentationController.IsHitBasedGoalTarget(targetTile))
                targetToGoalMovementPresentationController.HandleHitBasedTarget(targetTile, delegate { });

        }

        public void ExecuteEffectOn(CellStack cellStack, Action onCompleted)
        {
            if (cellStack.Top() is EmptyCell)
                onCompleted();
            else
                cellStack.Top().GetComponent<Core.CellPresenter>().PlayHitAnimation(onCompleted);
            
        }

        // NOTE: This should be refactored when attachment hitting is fully defined.
        public void ExecuteEffectOn(HitableCellAttachment cellAttachment, Action onCompleted)
        {
            if (cellAttachment is Rope rope)
                rope.GetComponent<RopePresenter>().PlayHitEffect(onCompleted);
            else
                onCompleted();
        }
    }
}