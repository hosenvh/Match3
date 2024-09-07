

using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Physics
{
    // TODO: Rename this.
    public class LemonadeSystem : GameplaySystem
    {

        List<CellStack> sinkCellStacks = new List<CellStack>();

        public LemonadeSystem(GameplayController gameplayController) : base(gameplayController)
        {
            foreach (var element in gameplayController.GameBoard().DefaultCellBoardIterator())
                if (element.value.HasAttachment<LemonadeSink>())
                    sinkCellStacks.Add(element.value);
        }

        public override void Update(float dt)
        {
            foreach (var cellStack in sinkCellStacks)
                if (HasLemonade(cellStack))
                    DestroyTheLemonadeIn(cellStack);
        }

        private void DestroyTheLemonadeIn(CellStack cellStack)
        {
            var topTile = cellStack.CurrentTileStack().Top();
            cellStack.CurrentTileStack().Pop();
            GetFrameData<DestructionData>().tilesToDestroy.Add(topTile);
        }

        bool HasLemonade(CellStack cellStack)
        {
            return cellStack.HasTileStack()
                && cellStack.CurrentTileStack().GetComponent<LockState>().IsFree()
                && cellStack.CurrentTileStack().IsDepleted() == false
                && cellStack.CurrentTileStack().Top() is Lemonade;
        }
    }
}