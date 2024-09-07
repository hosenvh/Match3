using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RiverMechanic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.RiverMechanic
{
    [After(typeof(General.UserMovementManagementSystem))]
    public class RiverBlockageDetectionSystem : GameplaySystem
    {
        private readonly HashSet<Type> blockingTypes = new HashSet<Type>() { typeof(Louie) };
        private GameBoard gameBoard;

        public RiverBlockageDetectionSystem(GameplayController gameplayController) : base(gameplayController)
        {
            this.gameBoard = gameplayController.GameBoard();
        }

        public override void Update(float dt)
        {
            ProcessBlockedTilesInFlowingRivers();
        }

        private void ProcessBlockedTilesInFlowingRivers()
        {
            foreach (var element in gameBoard.DefaultCellBoardIterator())
            {
                var riverCell = TryGetRiverCellIn(element.value);
                if (riverCell != null && HasBlockingType(element.value))
                {
                    AddToBlockedRiverCells(riverCell);
                }
            }
        }

        private RiverCell TryGetRiverCellIn(CellStack cellStack)
        {
            foreach (var cell in cellStack.Stack())
                if (cell is RiverCell)
                    return cell as RiverCell;

            return null;
        }

        private bool HasBlockingType(CellStack cellStack)
        {
            foreach (Type type in blockingTypes)
                if (HasTile(cellStack, type))
                    return true;
            
            return false;
        }

        private void AddToBlockedRiverCells(RiverCell riverCell)
        {
            GetFrameData<RiverBlockingData>().BlockedRiverCells.Add(riverCell);
        }
    }
}