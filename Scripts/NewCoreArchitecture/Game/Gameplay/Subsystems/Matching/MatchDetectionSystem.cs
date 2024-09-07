using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using System;
using System.Collections.Generic;
using System.Linq;
using static Match3.Game.Gameplay.QueryUtilities;
using static Match3.Game.Gameplay.ActionUtilites;

namespace Match3.Game.Gameplay.Matching
{
    // TODO: Refactor this shit.
    public class MatchDetectionSystem : GameplaySystem
    {

        MaximallyMergedMatchesCalculator maximallyMergedMatchesCalculator = new MaximallyMergedMatchesCalculator();
        CellStackBoard cellStackBoard;

        MatchingRulesTable matchingRulesTable;

        CellStack[] cellStacks;
        int[] rightCellStackIndexes;
        int[] topCellStacksIndexes;

        HashSet<CellStack> verticalVisitedCells = new HashSet<CellStack>();
        HashSet<CellStack> horizontalVisitedCells = new HashSet<CellStack>();

        Dictionary<Direction, List<Match>> matches = new Dictionary<Direction, List<Match>>(new DirectionComparer());
        HashSet<Match> lockedMatches = new HashSet<Match>();



        Match __TEMP_MATCH__;

        int boardWidth;

        public MatchDetectionSystem(GameplayController gameplayController) : base(gameplayController)
        {
            this.cellStackBoard = gameplayController.GameBoard().CellStackBoard();
            this.gameplayController = gameplayController;
            matchingRulesTable = gameplayController.matchingRulesTable;

            cellStacks = gameplayController.GameBoard().leftToRightTopDownCellStackArray;

            rightCellStackIndexes = new int[cellStacks.Length];
            topCellStacksIndexes = new int[cellStacks.Length];

            for (int i = 0; i < cellStacks.Length; ++i)
            {
                var pos = cellStacks[i].Position();
                var right = cellStackBoard.DirectionalElementOf(pos.x, pos.y, Direction.Right);
                var top = cellStackBoard.DirectionalElementOf(pos.x, pos.y, Direction.Up);
                rightCellStackIndexes[i] = right != null? cellStackBoard.PositionToLinearIndex(right.Position().x, right.Position().y) : -1;
                topCellStacksIndexes[i] = top != null ? cellStackBoard.PositionToLinearIndex(top.Position().x, top.Position().y) : -1;
            }


            matches[Direction.Right] = new List<Match>();
            matches[Direction.Up] = new List<Match>();

            boardWidth = cellStackBoard.Width();
            __TEMP_MATCH__ = new Match();
        }

        public override void Update(float dt)
        {
            horizontalVisitedCells.Clear();
            verticalVisitedCells.Clear();
            lockedMatches.Clear();


            matches[Direction.Right].Clear();
            matches[Direction.Up].Clear();


            for (int i = 0; i < cellStacks.Length; ++i)
                CreateHorizontalAndVerticalMatches(i);

            var finalMatches = maximallyMergedMatchesCalculator.Calculate(matches[Direction.Right], matches[Direction.Up], lockedMatches);

            foreach (var match in finalMatches)
               GetFrameData<CreatedMatchesData>().data.Add(match);

        }


        private void CreateHorizontalAndVerticalMatches(int index)
        {
            var cellStack = cellStacks[index];
            if (cellStack.HasTileStack() == false)
                return;

            var positon = cellStack.Position();

            if (boardWidth - positon.x >= 3)
                CreateLongestDirectionalMatchFrom(index, ref rightCellStackIndexes, ref horizontalVisitedCells, Direction.Right);
            if (positon.y >= 2)
                CreateLongestDirectionalMatchFrom(index, ref topCellStacksIndexes, ref verticalVisitedCells, Direction.Up);
        }


        private void CreateLongestDirectionalMatchFrom(int index, ref int[] directionalArray, ref HashSet<CellStack> visitedCellStacks, Direction direction)
        {
            var cellStack = cellStacks[index];
            if (visitedCellStacks.Contains(cellStack))
                return;

            if(__TEMP_MATCH__.tileStacks.Count > 0)
                __TEMP_MATCH__.tileStacks.Clear();
            FindLongestMatch(index, ref directionalArray, ref __TEMP_MATCH__);

            foreach (var t in __TEMP_MATCH__.tileStacks)
                visitedCellStacks.Add(t.Parent());


            if (IsMatchLengthSatisfied(__TEMP_MATCH__))
            {
                var match = new Match(__TEMP_MATCH__);
                matches[direction].Add(match);
                if (HasLockedTileStack(match))
                    lockedMatches.Add(match);
            }


        }

        bool HasLockedTileStack(Match match)
        {
            foreach(var tileStack in match.tileStacks)
                if(IsFullyFree(tileStack) == false)
                    return true;

            return false;
                
        }

        // TODO: Refactor this.
        private void FindLongestMatch(int index, ref int[] directionalArray, ref Match match)
        {
            var cellStack = cellStacks[index];

            TileStack currentTileStack = cellStack.CurrentTileStack();
            if (IsNotValidForMatching(currentTileStack))
                return;
            TileStack nextTileStack = null;

            var currentIndex = index;
            int nextIndex;

            while (true)
            {
                nextIndex = directionalArray[currentIndex];
                if (nextIndex == -1)
                    break;

                var nextCellStack = cellStacks[nextIndex];


                nextTileStack = nextCellStack.CurrentTileStack();
                if (IsNotValidForMatching(nextTileStack))
                    break;

                if (matchingRulesTable.DoesMatch(currentTileStack, nextTileStack))
                    match.Add(nextTileStack);
                else
                    break;
                currentTileStack = nextTileStack;
                currentIndex = nextIndex;
            }

            if (match.tileStacks.Count > 0)
                match.Add(cellStack.CurrentTileStack());
        }

        bool IsNotValidForMatching(TileStack tileStack)
        {
            return
                tileStack == null
                || (
                tileStack.componentCache.lockState.IsLocked()
                && tileStack.componentCache.lockState.IsLockedBy<Physics.PhysicsKeyType>() == false);
        }

        bool IsMatchLengthSatisfied(Match match)
        {
            return match.tileStacks.Count >= 3;
        }


    }
}