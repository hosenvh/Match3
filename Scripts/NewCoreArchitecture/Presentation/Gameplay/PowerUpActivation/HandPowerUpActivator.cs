

using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;
using Match3.Game.Gameplay.Swapping;
using Match3.Presentation.Gameplay.Core;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public class HandPowerUpActivator : PowerUpActivator
    {

        CellStackPresenter firstSelection;

        public override bool AcceptsSelection(CellStackPresenter cellStackPresenter)
        {
            var cellStack = cellStackPresenter.CellStack();
            if (cellStack.HasTileStack() == false || cellStack.CurrentTileStack().IsDepleted())
                return false;

            return cellStack.CurrentTileStack().Top().GetComponent<TileUserInteractionProperties>().isSwappable;
        }

        public override void OnCellStackSelected(CellStackPresenter target)
        {
            if (firstSelection == null || firstSelection == target)
                firstSelection = target;
            else if (AreAdjacent(firstSelection, target))
                Execute(new HandPowerUpInputCommand(firstSelection.CellStack(), target.CellStack()));
            else
                firstSelection = target;
        }

        public void OnCellStackSelectedByDrag(CellStackPresenter target, Direction dir)
        {
            var pos = target.CellStack().Position();
            var destination = gameplayController.GameBoard().DirectionalCellStackOf(pos.x, pos.y, dir);

            if(destination != null)
                Execute(new HandPowerUpInputCommand(target.CellStack(), destination));
        }

        protected override void ResetActivationState()
        {
            firstSelection = null;
        }

        private bool AreAdjacent(CellStackPresenter first, CellStackPresenter second)
        {
            return gameplayController.GameBoard().CellStackBoard().AreAdjacent(
                first.CellStack().Position(),
                second.CellStack().Position());
        }
    }
}