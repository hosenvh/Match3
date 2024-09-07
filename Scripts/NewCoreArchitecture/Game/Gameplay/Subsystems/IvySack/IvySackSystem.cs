using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.IvyMechanic;
using Match3.Game.Gameplay.TileGeneration;
using Match3.Game.Gameplay.Tiles;
using UnityEngine;
using static Match3.Game.Gameplay.QueryUtilities;


namespace Match3.Game.Gameplay.Subsystems.IvySack
{
    [Before(typeof(IvyExpansionSystem))]
    public class IvySackSystem : GameplaySystem
    {
        private readonly GameBoard gameBoard;
        private readonly IvyExpansionPresentationHandler expansionPresentationHandler;
        private readonly CellFactory cellFactory;
        private readonly TileFactory tileFactory;

        private readonly HashSet<IvySackTile> ivySacks = new HashSet<IvySackTile>();
        private readonly List<Direction> directions = new List<Direction> {Direction.Up, Direction.Down, Direction.Left, Direction.Right};
        private TileSourceSystem tileSourceSystem;


        public IvySackSystem(GameplayController gameplayController) : base(gameplayController)
        {
            gameBoard = gameplayController.GameBoard();
            expansionPresentationHandler = gameplayController.GetPresentationHandler<IvyExpansionPresentationHandler>();
            cellFactory = ServiceLocator.Find<CellFactory>();
            tileFactory = ServiceLocator.Find<TileFactory>();

            FetchIvySacksInBoard();
        }

        public override void Start()
        {
            tileSourceSystem = gameplayController.GetSystem<TileSourceSystem>();
            base.Start();
        }

        private void FetchIvySacksInBoard()
        {
            foreach (CellStack cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTile<IvySackTile>(cellStack))
                    ivySacks.Add(FindTile<IvySackTile>(cellStack));
        }

        public override void Update(float dt)
        {
            ProcessReadyForGenerationIvySacks();
        }

        private void ProcessReadyForGenerationIvySacks()
        {
            List<IvySackTile> ivySackTiles = new List<IvySackTile>(ivySacks);
            foreach (var ivySack in ivySackTiles)
            {
                if (ivySack.ShouldGetReadyForGeneration())
                {
                    ProcessReadyToGenerateIvySack(ivySack);
                    DestroyIvySack(ivySack);
                }
            }
        }

        private void ProcessReadyToGenerateIvySack(IvySackTile ivySackTile)
        {
            TryToGenerateIvyRootAroundIvySack(ivySackTile);
            TryGenerateIvyRootUnderIvySack(ivySackTile);
            GenerateColoredBeadTileAndIvyBushTileInIvySack(ivySackTile);
        }

        private void TryToGenerateIvyRootAroundIvySack(IvySackTile ivySackTile)
        {
            Vector2Int pos = ivySackTile.Parent().Parent().Position();

            foreach (var direction in directions)
            {
                CellStack targetCellStack = gameBoard.DirectionalCellStackOf(pos, direction);
                if (targetCellStack != null && IsValidToChoose(targetCellStack))
                    CreateIvyRootCellIn(targetCellStack);
            }
        }

        private bool IsValidToChoose(CellStack cellStack)
        {
            if (cellStack == null || CanGrowRootIn(cellStack) == false)
                return false;

            if (cellStack.CurrentTileStack() != null
                && cellStack.CurrentTileStack().IsDepleted() == false
                && HasCellOnTop<IvyRootCell>(cellStack))
                return false;

            return true;
        }

        private bool CanGrowRootIn(CellStack cellStack)
        {
            return cellStack.Top().GetComponent<IvyMechanicCellProperties>().canBeTakenOverByIvy
                   && (HasAnyTile(cellStack) == false || TopTile(cellStack).GetComponent<IvyMechanicTileProperties>().canBeTakenOverByIvy);
        }

        private void CreateIvyRootCellIn(CellStack targetCellStack)
        {
            gameplayController.creationUtility.PlaceCellInBoard(cellFactory.CreateIvyRootCell(), targetCellStack);
            expansionPresentationHandler.ShowRootGrowIn(targetCellStack);
        }

        private void TryGenerateIvyRootUnderIvySack(IvySackTile ivySackTile)
        {
            if (CanGrowRootIn(ivySackTile.Parent().Parent()))
                CreateIvyRootCellIn(ivySackTile.Parent().Parent());
        }

        private void GenerateColoredBeadTileAndIvyBushTileInIvySack(IvySackTile ivySackTile)
        {
            CreateColoredBeadTileAndIvyBushTileIn(ivySackTile.Parent().Parent());
        }

        private void CreateColoredBeadTileAndIvyBushTileIn(CellStack targetCellStack)
        {
            if (targetCellStack.CurrentTileStack().IsDepleted())
                Debug.LogError("CreateTileIn Target for ivySack is empty");

            gameplayController.creationUtility.ReplaceTileInBoard(tileFactory.CreateCleanColoredBeadTile(tileSourceSystem.ChooseAColor()), targetCellStack);
            gameplayController.creationUtility.PlaceTileInBoard(tileFactory.CreateIvyBushTile(), targetCellStack);
            expansionPresentationHandler.ShowBushGrowIn(targetCellStack);
        }

        private void DestroyIvySack(IvySackTile ivySackTile)
        {
            ivySacks.Remove(ivySackTile);
            ivySackTile.ForceDestroy();
            ivySackTile.MarkAsDestroyed();
        }
    }
}