using KitchenParadise.Utiltiy.Base;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.TileRandomPlacement;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.SubSystemsData.SessionData.General;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.HintingAndShuffling
{
    public struct ShufflingKeyType : KeyType
    {

    }

    public interface BoardShufflingPresentationHandler : PresentationHandler
    {
        void Shuffle(List<BoardShufflingSystem.ShuffleData> shuffles, Action onCompleted);
    }


    public struct ShufflingBoardEvent : GameEvent
    { }

    // TODO: Refactor this shit.
    public class BoardShufflingSystem : GameplaySystem
    {
        const int MAX_SHUFFLE_COUNT = 4;

        public struct ShuffleData
        {
            public readonly TileStack origin;
            public readonly CellStack destination;

            public ShuffleData(TileStack origin, CellStack destination)
            {
                this.origin = origin;
                this.destination = destination;
            }
        }


        BoardStabilityCalculator boardStabilityCalculator;
        PossibleMovesData possibleMovesData;
        StabilityData stabilityData;
        BoardShufflingPresentationHandler presentationHandler;


        int currentShufflesCount;
        List<CellStack> cellStacksToShuffle = new List<CellStack>();
        List<ShuffleData> shuffles = new List<ShuffleData>();

        CellStack[] cellStacks;

        bool isPlacingRocket = false;

        public BoardShufflingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            possibleMovesData = GetSessionData<PossibleMovesData>();
            stabilityData = GetFrameData<StabilityData>();
            cellStacks = gameplayController.GameBoard().leftToRightTopDownCellStackArray;
            boardStabilityCalculator = gameplayController.boardStabilityCalculator;
            presentationHandler = gameplayController.GetPresentationHandler<BoardShufflingPresentationHandler>();
        }

        public override void Update(float dt)
        {
            if (possibleMovesData.isPossibleMoveAvailable == false && stabilityData.wasStableLastChecked == true && boardStabilityCalculator.CalculateIsStable())
            {
                stabilityData.wasStableLastChecked = false;
                TryFixBoard();
            }
            if (possibleMovesData.isPossibleMoveAvailable)
                currentShufflesCount = 0;

        }

        private void TryFixBoard()
        {
            if (currentShufflesCount >= MAX_SHUFFLE_COUNT)
                PlaceARocket();
            else
                StartShufflingBoard();
        }

        private void PlaceARocket()
        {
            if (isPlacingRocket)
                return;

            isPlacingRocket = true;
            stabilityData.wasStableLastChecked = false;
            GetSessionData<InputControlData>().AddLockedBy<ShufflingKeyType>();

            var chosenCell = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard()).ChooseOne(s => HasColoredBead(s));
            var tile = ServiceLocator.Find<TileFactory>().CreateRocketTile();


            gameplayController.GetPresentationHandler<DefaultTilePlacerPresentationHandler>().
                PlaceSingle(tile, chosenCell, () => FinishPlacing(tile, chosenCell));
        }

        private void FinishPlacing(Tile tile, CellStack chosenCell)
        {
            isPlacingRocket = false;
            GetSessionData<InputControlData>().RemoveLockedBy<ShufflingKeyType>();

            gameplayController.creationUtility.ReplaceTileInBoard(tile, chosenCell);
        }

        private void StartShufflingBoard()
        {
            stabilityData.wasStableLastChecked = false;
            GetSessionData<InputControlData>().AddLockedBy<ShufflingKeyType>();

            currentShufflesCount += 1;

            foreach (var cellStack in cellStacks)
                if (HasColoredBead(cellStack))
                    cellStacksToShuffle.Add(cellStack);

            cellStacksToShuffle.Shuffle();

            for (int i = 0; i < cellStacksToShuffle.Count - 1; ++i)
            {
                shuffles.Add(new ShuffleData(cellStacksToShuffle[i].CurrentTileStack(), cellStacksToShuffle[i + 1]));

                cellStacksToShuffle[i].CurrentTileStack().GetComponent<LockState>().LockBy<ShufflingKeyType>();
            }
            
            shuffles.Add(new ShuffleData(cellStacksToShuffle[cellStacksToShuffle.Count - 1].CurrentTileStack(), cellStacksToShuffle[0]));
            cellStacksToShuffle[cellStacksToShuffle.Count - 1].CurrentTileStack().GetComponent<LockState>().LockBy<ShufflingKeyType>();

            presentationHandler.Shuffle(shuffles, ApplyShuffles);
            cellStacksToShuffle.Clear();

            ServiceLocator.Find<EventManager>().Propagate(new ShufflingBoardEvent(), this);
        }

        private void ApplyShuffles()
        {
            GetSessionData<InputControlData>().RemoveLockedBy<ShufflingKeyType>();

            foreach (var shuffle in shuffles)
            {
                shuffle.origin.GetComponent<LockState>().Release();

                shuffle.destination.SetCurrnetTileStack(shuffle.origin);
                shuffle.origin.SetPosition(shuffle.destination.Position());

            }

            shuffles.Clear();
        }


        private bool HasColoredBead(CellStack cellStack)
        {
            var tileStack = cellStack.CurrentTileStack();

            return tileStack != null && tileStack.IsDepleted() == false && tileStack.Top() is ColoredBead;
        }
    }
}