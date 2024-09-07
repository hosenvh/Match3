

using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.ArtifactMechanic;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.SubSystems.GrassSackMechanic
{
    public struct GrassSackOpeningKeyType : KeyType
    {}

    public interface GrassSackOpeningPort : PresentationHandler
    {
        void ProcessGrassCreatingIn(CellStack cellStack, int distanceFromOrigin, Action<CellStack> onCompleted);
        void ProcessArtifactOpening(ArtifactMainCell artifact, Action<ArtifactMainCell> onCompleted);
    }

    [After(typeof(DestructionSystem))]
    public class GrassSackOpeningSystem : GameplaySystem
    {
        CellFactory cellFactory;
        CreationController creationController;
        DestroyedObjectsData destroyedObjectsData;
        GrassSackOpeningPort grassSackOpeningPort;
        GrassSackAreaHandler grassSackAreaHandler;
        CellStackBoard cellStackBoard;



        public GrassSackOpeningSystem(
            GameplayController gameplayController, 
            CellFactory cellFactory, 
            CreationController creationController) : base(gameplayController)
        {
            this.cellFactory = cellFactory;
            this.creationController = creationController;
            this.destroyedObjectsData = GetFrameData<DestroyedObjectsData>();
            this.cellStackBoard = gameplayController.GameBoard().CellStackBoard();
            this.grassSackAreaHandler = new GrassSackAreaHandler(cellStackBoard);
        }

        public override void Start()
        {
            grassSackOpeningPort = gameplayController.GetPresentationHandler<GrassSackOpeningPort>();
        }

        public override void Update(float dt)
        {
            foreach (var tile in destroyedObjectsData.tiles)
                if (tile is GrassSackMainTile grassSackMainTile)
                    ProcessOpeningUp(grassSackMainTile, tile.GetComponent<TileDestructionExtraInfo>().parent.Position());
        }

        private void ProcessOpeningUp(GrassSackMainTile grassSackMainTile, Vector2Int sackOrigin)
        {
            ProcessArtifactCreation(grassSackMainTile);
            ProcessGrassCreation(sackOrigin);
        }

        private void ProcessArtifactCreation(GrassSackMainTile grassSackMainTile)
        {
            foreach(var data in grassSackMainTile.ArtifactsData())
            {
                var cellStack = cellStackBoard [cellStackBoard.LinearIndexToPosition(data.positionIndex)];
                var artifactCell = cellFactory.CreateMainArtifactCell(data.direction, data.size) as ArtifactMainCell;

                creationController.PlaceCellInBoard(artifactCell, cellStack);

                MoveGrassesToTopOf(artifactCell);

                grassSackOpeningPort.ProcessArtifactOpening(artifactCell, AddArtifactToDiscoverySystem);
            }
        }

        private void ProcessGrassCreation(Vector2Int sackOrigin)
        {
            grassSackAreaHandler.ForEachInnerCellStackDo(sackOrigin, action: ProcessTwoLevelGrassOpening);
            grassSackAreaHandler.ForEachOuterCellStackDo(sackOrigin, action: ProcessOneLevelGrassOpening);
        }

        private void MoveGrassesToTopOf(ArtifactMainCell artifactCell)
        {
            var cellStack = artifactCell.Parent();
            var grassCell = QueryUtilities.FindCell<GrassCell>(cellStack);
            if (grassCell != null)
                creationController.MoveToTop(grassCell);

            foreach (var slave in artifactCell.Slaves())
            {
                grassCell = QueryUtilities.FindCell<GrassCell>(slave.Parent());
                if (grassCell != null)
                    creationController.MoveToTop(grassCell);
            }
        }

        private void AddArtifactToDiscoverySystem(ArtifactMainCell artifactMainCell)
        {
            // TODO: Please find a better way and remove this dependency
            gameplayController.GetSystem<ArtifactDiscoverySystem>().TryAddArtifactIn(artifactMainCell.Parent());
        }

        private void ProcessTwoLevelGrassOpening(CellStack cellStack, int distanceFromSack)
        {
            ProcessGrassOpening(cellStack, 2, distanceFromSack);
        }

        private void ProcessOneLevelGrassOpening(CellStack cellStack, int distanceFromSack)
        {
            ProcessGrassOpening(cellStack, 1, distanceFromSack);
        }

        private void ProcessGrassOpening(CellStack cellStack, int levelUpAmount, int distanceFromSack)
        {
            grassSackOpeningPort.ProcessGrassCreatingIn(cellStack, distanceFromSack, onCompleted: (stack) => ApplyOpening(stack, levelUpAmount));
 
        }

        private void ApplyOpening(CellStack cellStack, int grassLevel)
        {
            var grassCell = QueryUtilities.FindCell<GrassCell>(cellStack);

            if (grassCell != null && grassCell.IsDestroyed() == false)
                grassCell.IncreaseLevel(grassLevel);
            else
                creationController.PlaceCellInBoard(cellFactory.CreateGrassCell(grassLevel), cellStack);
        }
    }
}