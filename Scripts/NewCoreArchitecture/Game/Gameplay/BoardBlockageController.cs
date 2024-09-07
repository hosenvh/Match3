using Match3.Game.Gameplay.Core;
using System.Linq;
using System.Collections.Generic;
using Match3.Game.Gameplay.CellAttachments;
using System;
using Match3.Utility.GolmoradLogging;


namespace Match3.Game.Gameplay
{
    public class BoardBlockageController
    {
        struct CellStackSurroundingData
        {
            public readonly CellStack cellStack;
            public readonly CellStack bottomCellStack;
            public readonly CellStack leftCellStack;

            public CellStackSurroundingData(CellStack cellStack, CellStack bottomCellStack, CellStack leftCellStack)
            {
                this.cellStack = cellStack;
                this.bottomCellStack = bottomCellStack;
                this.leftCellStack = leftCellStack;
            }
        }

        Dictionary<Direction, HashSet<CellStack>> wallBlockages = new Dictionary<Direction, HashSet<CellStack>>(new DirectionComparer());
        Dictionary<Direction, HashSet<CellStack>> ropeBlockages = new Dictionary<Direction, HashSet<CellStack>>(new DirectionComparer());

        CellStackBoard cellStackBoard;
        GameBoard gameBoard;

        List<Wall> temporaryWallsContainer = new List<Wall>();
        List<Rope> temporaryRopesContainer = new List<Rope>();

        public BoardBlockageController(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
            this.cellStackBoard = gameBoard.CellStackBoard();

            InitializeBlockagesDictionary();
            DetermineBlockages();
        }

        public bool IsBlocked(CellStack cellStack, Direction direction)
        {
            // NOTE: This can be cached for optimization.
            return wallBlockages[direction].Contains(cellStack) || ropeBlockages[direction].Contains(cellStack);
        }

        public bool CanNotSideFallFromTopLeft(CellStack cellStack)
        {
            var pos = cellStack.Position();
            var topCell = cellStackBoard.DirectionalElementOf(pos, Direction.Up);
            var leftCell = cellStackBoard.DirectionalElementOf(pos, Direction.Left);

            return
                (IsBlocked(cellStack, Direction.Left) && IsBlocked(topCell, Direction.Left))
                || (IsBlocked(cellStack, Direction.Left) && IsBlocked(cellStack, Direction.Up))
                || (IsBlocked(leftCell, Direction.Up) && IsBlocked(topCell, Direction.Left))
                || (IsBlocked(cellStack, Direction.Up) && IsBlocked(leftCell, Direction.Up));

        }

        public bool CanNotSideFallFromTopRight(CellStack cellStack)
        {
            var pos = cellStack.Position();
            var topCell = cellStackBoard.DirectionalElementOf(pos, Direction.Up);
            var rightCell = cellStackBoard.DirectionalElementOf(pos, Direction.Right);

            return
                (IsBlocked(cellStack, Direction.Right) && IsBlocked(topCell, Direction.Right))
                || (IsBlocked(cellStack, Direction.Right) && IsBlocked(cellStack, Direction.Up))
                || (IsBlocked(rightCell, Direction.Up) && IsBlocked(topCell, Direction.Right))
                || (IsBlocked(cellStack, Direction.Up) && IsBlocked(rightCell, Direction.Up));

        }

        public void UpdateRopeBlockageFor(CellStack owner)
        {
            var surroundingData = CreateSurroundingDataFor(owner);
            ropeBlockages[Direction.Up].Remove(surroundingData.bottomCellStack);
            ropeBlockages[Direction.Down].Remove(surroundingData.cellStack);

            ropeBlockages[Direction.Right].Remove(surroundingData.leftCellStack);
            ropeBlockages[Direction.Left].Remove(surroundingData.cellStack);

            DetermineRopeBlockages(surroundingData, ref temporaryRopesContainer);
        }

        private void InitializeBlockagesDictionary()
        {
            wallBlockages[Direction.Up] = new HashSet<CellStack>();
            wallBlockages[Direction.Down] = new HashSet<CellStack>();
            wallBlockages[Direction.Left] = new HashSet<CellStack>();
            wallBlockages[Direction.Right] = new HashSet<CellStack>();

            ropeBlockages[Direction.Up] = new HashSet<CellStack>();
            ropeBlockages[Direction.Down] = new HashSet<CellStack>();
            ropeBlockages[Direction.Left] = new HashSet<CellStack>();
            ropeBlockages[Direction.Right] = new HashSet<CellStack>();
        }


        private void DetermineBlockages()
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                var surroundingData = CreateSurroundingDataFor(cellStack);

                DetermineWallBlockages(surroundingData, ref temporaryWallsContainer);
                DetermineRopeBlockages(surroundingData, ref temporaryRopesContainer);
            }
        }

        private void DetermineWallBlockages(CellStackSurroundingData surroundingData, ref List<Wall> tempWalls)
        {
            tempWalls.Clear();
            surroundingData.cellStack.GetAttachments<Wall>(ref tempWalls);

            foreach (var wall in tempWalls)
                DetermineBlockageBasedOnPlacement(wall.placement, surroundingData, ref wallBlockages);
        }

        private void DetermineRopeBlockages(CellStackSurroundingData surroundingData, ref List<Rope> tempRopes)
        {
            tempRopes.Clear();
            surroundingData.cellStack.GetAttachments<Rope>(ref tempRopes);

            foreach (var rope in tempRopes)
                DetermineBlockageBasedOnPlacement(rope.placement, surroundingData, ref ropeBlockages);
        }

        private void DetermineBlockageBasedOnPlacement(GridPlacement placement, CellStackSurroundingData surroundingData, ref Dictionary<Direction, HashSet<CellStack>> blockages)
        {
            switch (placement)
            {
                case GridPlacement.Down:
                    TryAddBlockage(surroundingData.cellStack, Direction.Down, ref blockages);
                    TryAddBlockage(surroundingData.bottomCellStack, Direction.Up, ref blockages);
                    break;
                case GridPlacement.Left:
                    TryAddBlockage(surroundingData.cellStack, Direction.Left, ref blockages);
                    TryAddBlockage(surroundingData.leftCellStack, Direction.Right, ref blockages);
                    break;
                default:
                    DebugPro.LogError<CoreGameplayLogTag>(message: $"Placement {placement} is not supported yet");
                    break;
            }
        }

        private CellStackSurroundingData CreateSurroundingDataFor(CellStack cellStack)
        {
            var pos = cellStack.Position();

            var bottomCellStack = cellStackBoard.DirectionalElementOf(pos, Direction.Down);
            var leftCellStack = cellStackBoard.DirectionalElementOf(pos, Direction.Left);

            return new CellStackSurroundingData(cellStack, bottomCellStack, leftCellStack);
        }

        private void TryAddBlockage(CellStack cellStack, Direction direction, ref Dictionary<Direction, HashSet<CellStack>> blockages)
        {
            if (cellStack != null)
                blockages[direction].Add(cellStack);
        }

    }
}