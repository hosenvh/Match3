using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.SessionData.General;
using Match3.Game.Gameplay.Swapping;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.SubSystems.HintingAndShuffling
{
    public interface HintingPresentationHandler : PresentationHandler
    {
        void Apply(Hint hint);
        void Stop();
    }

    // TODO: Refactor this.
    public class HintingSystem : GameplaySystem
    {
        private enum State { HintReady, HintNotReady }

        public struct Move
        {
            public readonly CellStack origin;
            public readonly CellStack destination;

            public Move(CellStack origin, CellStack destination)
            {
                this.origin = origin;
                this.destination = destination;
            }
        }

        private readonly CellStack[] cellStacksLeftToRightBottomUp;
        private readonly List<Move> potentialMoves;
        private readonly CellStackBoard cellStackBoard;
        private readonly StabilityData stabilityData;
        private State state;

        private readonly HintingPresentationHandler hintingPresentationHandler;

        private Hint manualHint = null;
        private List<CellStack> matchHintCellStacksCache = new List<CellStack>();

        public HintingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            // NOTE: The direction is important.
            cellStacksLeftToRightBottomUp = gameplayController.GameBoard().leftToRightButtomUpCellStackArray;
            potentialMoves = new List<Move>();
            cellStackBoard = gameplayController.GameBoard().CellStackBoard();
            stabilityData = GetFrameData<StabilityData>();
            hintingPresentationHandler = gameplayController.GetPresentationHandler<HintingPresentationHandler>();

            state = State.HintNotReady;
        }

        public override void Reset()
        {
            state = State.HintNotReady;
            manualHint = null;
        }

        public override void Update(float dt)
        {
            switch (state)
            {
                case State.HintReady:
                    ProcessHintReadyState();
                    break;
                case State.HintNotReady:
                    ProcessHintNotReadyState();
                    break;
            }
        }

        private void ProcessHintNotReadyState()
        {
            if(stabilityData.wasStableLastChecked == true)
            {
                var hint = manualHint;
                if(hint == null)
                    hint = DetermineAHint();

                if(hint != null)
                {
                    hintingPresentationHandler.Apply(hint);
                    state = State.HintReady;
                    GetSessionData<PossibleMovesData>().isPossibleMoveAvailable = true;
                }
            }
        }

        public void SetManualHint(Hint hint)
        {
            manualHint = hint;
            state = State.HintNotReady;
            hintingPresentationHandler.Stop();
            GetSessionData<PossibleMovesData>().isPossibleMoveAvailable = false;
        }

        private void ProcessHintReadyState()
        {
            if (stabilityData.wasStableLastChecked == false)
            {
                state = State.HintNotReady;
                hintingPresentationHandler.Stop();
                GetSessionData<PossibleMovesData>().isPossibleMoveAvailable = false;
            }
        }

        private Hint DetermineAHint()
        {
            foreach (var cell in cellStacksLeftToRightBottomUp)
            {
                Hint hint = null;

                if (hint == null) hint = TryCreateExplosiveHint(cell);
                if (hint == null) hint = TryCreateRainbowHint(cell);
                if (hint == null) hint = DetermineMatchHint(cell);

                if (hint != null)
                    return hint;
            }

            return null;
        }

        private Hint TryCreateExplosiveHint(CellStack cell)
        {
            if (TopIs<ExplosiveTile>(cell))
                return new ActivationHint(cell);

            return null;
        }

        private Hint TryCreateRainbowHint(CellStack cell)
        {
            if (TopIs<Rainbow>(cell) == false)
                return null;

            int x = cell.Position().x;
            int y = cell.Position().y;
            var top = cellStackBoard[x, y - 1];
            var bottom = cellStackBoard[x, y + 1];
            var left = cellStackBoard[x - 1, y];
            var right = cellStackBoard[x + 1, y];


            if (IsRainbowMatchable(top) && AreSwappable(cell, top))
                return new MoveHint(cell, top);
            if (IsRainbowMatchable(bottom) && AreSwappable(cell, bottom))
                return new MoveHint(cell, bottom);
            if (IsRainbowMatchable(left) && AreSwappable(cell, left))
                return new MoveHint(cell, left);
            if (IsRainbowMatchable(right) && AreSwappable(cell, right))
                return new MoveHint(cell, right);

            return null;
        }


        // TODO: Refactor this shit.
        private Hint DetermineMatchHint(CellStack cell)
        {
            potentialMoves.Clear();

            int x = cell.Position().x;
            int y = cell.Position().y;

            var rightCell = cellStackBoard[x + 1, y];
            var topCell = cellStackBoard[x, y - 1];

            var rightRightCell = cellStackBoard[x + 2, y];
            var topTopCell = cellStackBoard[x, y - 2];

            if (DoesMatch(cell, rightCell))
            {
                TryAddPotentialMoves(cellStackBoard[x - 2, y], cellStackBoard[x - 1, y]);
                TryAddPotentialMoves(cellStackBoard[x - 1, y + 1], cellStackBoard[x - 1, y]);
                TryAddPotentialMoves(cellStackBoard[x - 1, y - 1], cellStackBoard[x - 1, y]);

                TryAddPotentialMoves(cellStackBoard[x + 3, y], cellStackBoard[x + 2, y]);
                TryAddPotentialMoves(cellStackBoard[x + 2, y + 1], cellStackBoard[x + 2, y]);
                TryAddPotentialMoves(cellStackBoard[x + 2, y - 1], cellStackBoard[x + 2, y]);
            }

            if (DoesMatch(cell, topCell))
            {
                TryAddPotentialMoves(cellStackBoard[x, y - 3], cellStackBoard[x, y - 2]);
                TryAddPotentialMoves(cellStackBoard[x + 1, y - 2], cellStackBoard[x, y - 2]);
                TryAddPotentialMoves(cellStackBoard[x - 1, y - 2], cellStackBoard[x, y - 2]);

                TryAddPotentialMoves(cellStackBoard[x, y + 2], cellStackBoard[x, y + 1]);
                TryAddPotentialMoves(cellStackBoard[x - 1, y + 1], cellStackBoard[x, y + 1]);
                TryAddPotentialMoves(cellStackBoard[x + 1, y + 1], cellStackBoard[x, y + 1]);
            }

            if (DoesMatch(cell, rightRightCell))
            {
                TryAddPotentialMoves(cellStackBoard[x + 1, y - 1], cellStackBoard[x + 1, y]);
                TryAddPotentialMoves(cellStackBoard[x + 1, y + 1], cellStackBoard[x + 1, y]);
            }

            if (DoesMatch(cell, topTopCell))
            {
                TryAddPotentialMoves(cellStackBoard[x - 1, y - 1], cellStackBoard[x, y - 1]);
                TryAddPotentialMoves(cellStackBoard[x + 1, y - 1], cellStackBoard[x, y - 1]);
            }

            foreach (var move in potentialMoves)
                if (DoesMatch(move.origin, cell) && AreSwappable(move.origin, move.destination))
                    return CreateMatchHint(move.origin, move.destination);


            return null;
        }

        public MatchHint CreateMatchHint(CellStack origin, CellStack destination)
        {
            matchHintCellStacksCache.Clear();

            var position = destination.Position();

            AddMatchesInDirection(position, new Vector2Int(0, 1), origin, ref matchHintCellStacksCache);
            AddMatchesInDirection(position, new Vector2Int(0, -1), origin, ref matchHintCellStacksCache);
            if (matchHintCellStacksCache.Count < 2)
                matchHintCellStacksCache.Clear();
            int addedCountInStraightLine = matchHintCellStacksCache.Count;
            AddMatchesInDirection(position, new Vector2Int(1, 0), origin, ref matchHintCellStacksCache);
            AddMatchesInDirection(position, new Vector2Int(-1, 0), origin, ref matchHintCellStacksCache);
            if (matchHintCellStacksCache.Count - addedCountInStraightLine < 2)
                matchHintCellStacksCache.RemoveRange(addedCountInStraightLine, matchHintCellStacksCache.Count - addedCountInStraightLine);

            // NOTE: giving matchHintCellStacksCache directly can be dangerous it maybe better to give a copy
            return new MatchHint(origin, destination, matchHintCellStacksCache);
        }

        private void AddMatchesInDirection(Vector2Int startPosition, Vector2Int direction, CellStack target,  ref List<CellStack> others)
        {
            CellStack cellStack = null;
            var currentPos = startPosition + direction;

            while(cellStackBoard.IsInRange(currentPos.x, currentPos.y))
            {
                cellStack = cellStackBoard[currentPos];

                if (cellStack == target || DoesMatch(cellStack, target) == false)
                    break;
                else
                    others.Add(cellStack);
   
                currentPos += direction;
            }

        }

        private bool AreSwappable(CellStack cellStack1, CellStack cellStack2)
        {
            return 
                IsValidForSwapping(cellStack1.CurrentTileStack())
                && IsValidForSwapping(cellStack2.CurrentTileStack())
                && IsThereWallBetween(cellStack1, cellStack2) == false;
        }

        private bool IsThereWallBetween(CellStack cellStack1, CellStack cellStack2)
        {
            var direction = cellStackBoard.RelativeDirectionOf(cellStack1.Position(), cellStack2.Position());

            return gameplayController.boardBlockageController.IsBlocked(cellStack1, direction);

        }

        private bool IsValidForSwapping(TileStack target)
        {
            return target != null
                && target.IsDepleted() == false
                && target.Top().GetComponent<TileUserInteractionProperties>().isSwappable;
        }

        private bool TopIs<T>(CellStack cell) where T : Tile
        {
            return
                cell.HasTileStack()
                && cell.CurrentTileStack().IsDepleted() == false
                && cell.CurrentTileStack().Top() is T;
        }

        private bool IsRainbowMatchable(CellStack cellStack)
        {
            if (cellStack == null)
                return false;

            var tileStack = cellStack.CurrentTileStack();

            return tileStack != null && tileStack.IsDepleted() == false && tileStack.Top().GetComponent<TileRainbowProperties>().DoesMatchWithRainbow();
        }


        private bool DoesMatch(CellStack cell1, CellStack cell2)
        {
            return cell1 != null && cell2 != null 
                && cell1.HasTileStack() && cell2.HasTileStack()
                && gameplayController.matchingRulesTable.DoesMatch(cell1.CurrentTileStack(), cell2.CurrentTileStack());
        }

        void TryAddPotentialMoves(CellStack origin, CellStack destination)
        {
            if (origin != null)
                potentialMoves.Add(new Move(origin, destination));
        }
    }
}