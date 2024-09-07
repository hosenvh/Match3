using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.Tiles;


namespace Match3.Game.Gameplay.SubSystems.TileRandomPlacement
{
    public interface TilePlacerPresentationHandler : PresentationHandler
    {
        void PlaceAll(List<TileToPlace> tilesToPlace, Action<TileToPlace> onEachTilePlaced, Action onCompleted);
    }

    public class TileToPlace
    {
        public readonly Tile tile;
        public readonly CellStack targetCellStack;

        public TileToPlace(Tile tile, CellStack targetCellStack)
        {
            this.tile = tile;
            this.targetCellStack = targetCellStack;
        }
    }

    public interface TileRandomPlacer
    {
        int Priority { get; }

        void Setup(GameplayController gameplayController);
        void AddTilesToPlace<T>() where T : Tile;
        void AddTilesToPlace(Type tileType);
        bool AnyTileToPlace();
        void StartTilePlacing(Action onComplete);
    }

    public abstract class TileRandomPlacer<TPresentationHandler> : TileRandomPlacer where TPresentationHandler : TilePlacerPresentationHandler
    {
        public int Priority { get; }

        protected readonly TPresentationHandler presentationHandler;
        protected GameplayController gameplayController;

        private readonly List<Type> nonRainbowsToPlace = new List<Type>();
        private readonly List<Type> rainbowsToPlace = new List<Type>();

        private readonly HashSet<CellStack> forbiddenCellStacks = new HashSet<CellStack>();

        public TileRandomPlacer(int priority, TPresentationHandler presentationHandler)
        {
            this.Priority = priority;
            this.presentationHandler = presentationHandler;
        }

        public void Setup(GameplayController gameplayController)
        {
            this.gameplayController = gameplayController;
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (cellStack.HasAttachment<RainbowPreventer>())
                    forbiddenCellStacks.Add(cellStack);
        }

        public void AddTilesToPlace<T>() where T : Tile
        {
            AddTilesToPlace(typeof(T));
        }

        public void AddTilesToPlace(Type tileType)
        {
            if (tileType == typeof(Rainbow))
                rainbowsToPlace.Add(tileType);
            else
                nonRainbowsToPlace.Add(tileType);
        }

        public bool AnyTileToPlace()
        {
            return nonRainbowsToPlace.Count != 0 || rainbowsToPlace.Count != 0;
        }

        public void StartTilePlacing(Action onComplete)
        {
            var rainbowCellStacks = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard())
               .Choose(rainbowsToPlace.Count, (c) => forbiddenCellStacks.Contains(c) == false && IsCellStackValid(c));

            var nonRainbowCellStacks = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard())
               .Choose(nonRainbowsToPlace.Count, (c) => rainbowCellStacks.Contains(c) == false && IsCellStackValid(c));

            var rainbowTiles = new List<Tile>();
            var nonRainbowTiles = new List<Tile>();

            for (int i = 0; i < rainbowCellStacks.Count; i++)
                rainbowTiles.Add(CreateTileFor(rainbowsToPlace[i]));

            for (int i = 0; i < nonRainbowCellStacks.Count; i++)
                nonRainbowTiles.Add(CreateTileFor(nonRainbowsToPlace[i]));

            List<TileToPlace> tilesToPlace = ConvertToTilesToPlace(rainbowTiles, nonRainbowTiles, rainbowCellStacks, nonRainbowCellStacks);

            if (tilesToPlace.Count == 0)
                Finish();
            else
                presentationHandler.PlaceAll(tilesToPlace, onEachTilePlaced: ApplyPlacing, onCompleted: Finish);

            void Finish()
            {
                FinishPlacing(); 
                onComplete.Invoke();
            }
        }

        private List<TileToPlace> ConvertToTilesToPlace(List<Tile> rainbowTiles, List<Tile> nonRainbowTiles, List<CellStack> rainbowCellStacks, List<CellStack> nonRainbowCellStacks)
        {
            List<TileToPlace> result = new List<TileToPlace>(rainbowTiles.Count + nonRainbowTiles.Count);

            for (int i = 0; i < rainbowTiles.Count; i++)
                result.Add(new TileToPlace(rainbowTiles[i], rainbowCellStacks[i]));

            for (int i = 0; i < nonRainbowTiles.Count; i++)
                result.Add(new TileToPlace(nonRainbowTiles[i], nonRainbowCellStacks[i]));

            return result;
        }

        private void ApplyPlacing(TileToPlace toPlace)
        {
            var cellStack = toPlace.targetCellStack;
            var tile = toPlace.tile;

            if (cellStack.HasTileStack())
            {
                cellStack.CurrentTileStack().Destroy();
            }

            var factory = ServiceLocator.Find<TileFactory>();
            var tileStack = factory.CreateTileStack();
            tileStack.Push(tile);

            cellStack.SetCurrnetTileStack(tileStack);
            tileStack.SetPosition(cellStack.Position());

            ServiceLocator.Find<EventManager>().Propagate(new TileStackGeneratedEvent(tileStack), this);
        }

        private void FinishPlacing()
        {
            nonRainbowsToPlace.Clear();
            rainbowsToPlace.Clear();
        }

        // TODO: Find a more extendable way.
        protected abstract Tile CreateTileFor(Type type);

        bool IsCellStackValid(CellStack cellStack)
        {
            return cellStack.HasTileStack()
                && cellStack.CurrentTileStack().IsDepleted() == false
                && cellStack.CurrentTileStack().Top() is ColoredBead
                && cellStack.CurrentTileStack().GetComponent<LockState>().IsFree();
        }
    }
}