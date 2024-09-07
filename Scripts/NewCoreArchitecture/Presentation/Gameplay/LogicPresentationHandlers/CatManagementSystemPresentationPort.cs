using System;
using DG.Tweening;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.CatMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class CatManagementSystemPresentationPort : PresentationController, CatManagementSystemPort
    {
        public AnimationCurve movementAnimationCurve;
        public float movementAnimationDuration;

        public SwappingPresentationHandlerImp swappingPresentationHandler;
        public GameplayStateRoot gameplayStateRoot;

        protected override void InternalSetup(GameplayState gameState)
        {
            var catSystem = gameState.gameplayController.GetSystem<CatManagementSystem>();
            if (catSystem.IsActive())
                SetInitialDirection(catSystem.TargetCat(), catSystem.CurrentPathCell());
        }

        private void SetInitialDirection(Cat cat, CatPathCell currentPathCell)
        {
            var direction = RelativeDirectionOf(currentPathCell.Parent(), currentPathCell.NextCell().Parent());
            cat.GetComponent<CatTilePresenter>().PlayIdle(direction);
        }


        public void Hit(Cat cat, CellStack target, Action onHit,  Action onComplete)
        {
            var direction = RelativeDirectionOf(cat, target);
            var presenter = cat.GetComponent<CatTilePresenter>();
            presenter.PlayAttackAnimation(
                direction, 
                onHit, 
                () => { presenter.PlayIdle(direction); onComplete.Invoke(); });
        }

        public void MoveCatToFood(Cat cat, CatFood catFood, Action onComplete)
        {
            var direction = RelativeDirectionOf(cat, catFood.Parent().Parent());

            var catPresenter = cat.GetComponent<CatTilePresenter>();
            var tileStackPresenter = cat.Parent().GetComponent<TileStackPresenter>();
            tileStackPresenter.SetLogicPositionUpdateFlag(false);

            catPresenter.PlayMoveAnimation(direction);

            tileStackPresenter.transform.
                DOMove(catFood.Parent().GetComponent<TileStackPresenter>().transform.position, movementAnimationDuration).
                SetEase(movementAnimationCurve).
                OnComplete(() => { tileStackPresenter.SetLogicPositionUpdateFlag(true); catPresenter.PlayIdle(direction); onComplete.Invoke(); });
        }


        public void SwapCat(Cat cat, CellStack target, Action onComplete)
        {
            var direction = RelativeDirectionOf(cat, target);
            var presenter = cat.GetComponent<CatTilePresenter>();
            presenter.PlayMoveAnimation(direction);

            swappingPresentationHandler.HandleSwap(
                cat.Parent().Parent(), 
                target, 
                movementAnimationDuration,
                movementAnimationCurve,
                () => { presenter.PlayIdle(direction); onComplete.Invoke(); });
        }

        private Direction RelativeDirectionOf(Cat cat, CellStack target)
        {
            return RelativeDirectionOf(cat.Parent().Parent(), target);
        }

        private Direction RelativeDirectionOf(CellStack origin, CellStack destination)
        {
            return gameplayStateRoot.gameplayState.gameplayController.
                GameBoard().RelativeDirectionOf(origin.Position(), destination.Position());
        }
    }
}