using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.HoneyMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.ActionUtilites;

namespace Match3.Game.Gameplay.SubSystems.IvyMechanic
{

    public interface IvyExpansionPresentationHandler : PresentationHandler
    {
        void ShowBushGrowIn(CellStack cellStack);
        void ShowRootGrowIn(CellStack cellStack);
    }

    public struct IvySystemKeyType : KeyType
    {
    }

    [After(typeof(DestructionManagement.DestructionSystem))]
    public class IvyExpansionSystem : GameplaySystem
    {
        UserMovementData userMovementData;

        IvyValidCellsToGrowUpdater ivyValidCellsUpdater;
        IvyBoardStateHandler ivyStateHandler;

        RandomCellStackChooser randomCellStackChooser;

        IvyExpansionPresentationHandler presentationHandler;

        public IvyExpansionSystem(GameplayController gameplayController) : base(gameplayController)
        {
            userMovementData = GetFrameData<UserMovementData>();
            randomCellStackChooser = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard());
            ivyStateHandler = new IvyBoardStateHandler(gameplayController.GameBoard(), GetFrameData<DestroyedObjectsData>());
            ivyValidCellsUpdater = new IvyValidCellsToGrowUpdater(gameplayController.GameBoard(), ivyStateHandler, gameplayController.boardBlockageController);
            presentationHandler = gameplayController.GetPresentationHandler<IvyExpansionPresentationHandler>();
        }

        public override void Start()
        {
            ivyStateHandler.UpdateIvyCellStacks();
            if (ivyStateHandler.ShouldBeDisable())
                gameplayController.DeactivateSystem<IvyExpansionSystem>();
        }

        public override void Update(float dt)
        {
            ivyStateHandler.TryUpdateIvyCellStacks();

            if(ivyStateHandler.ShouldBeDisable())
            {
                gameplayController.DeactivateSystem<IvyExpansionSystem>();
                return;
            }

            if(userMovementData.moves > 0 && ivyStateHandler.CanGrow())
                StartGrowingTheIvy();
        }

        private void StartGrowingTheIvy()
        {
            ivyValidCellsUpdater.UpdateValidCellStackForGrowing();

            var random = UnityEngine.Random.Range(0,3);

            switch (random)
            {
                case 0:
                    ApplyGrowthIn(TryChooseOneFromRoots());
                    ApplyGrowthIn(TryChooseOneFromBushes());
                    break;

                case 1:
                    ApplyGrowthIn(TryChooseOneFromRoots());
                    ApplyGrowthIn(TryChooseOneFromRoots());
                    break;
                case 2:
                    ApplyGrowthIn(TryChooseOneFromBushes());
                    ApplyGrowthIn(TryChooseOneFromBushes());
                    break;

            }
        }

        private CellStack TryChooseOneFromBushes()
        {
            return randomCellStackChooser.ChooseOne(ivyValidCellsUpdater.IsCellStackValidForGrowingBush)
                ?? randomCellStackChooser.ChooseOne(ivyValidCellsUpdater.IsCellStackValidForGrowingRoot);
        }

        private CellStack TryChooseOneFromRoots()
        {
            return randomCellStackChooser.ChooseOne(ivyValidCellsUpdater.IsCellStackValidForGrowingRoot)
                ?? randomCellStackChooser.ChooseOne(ivyValidCellsUpdater.IsCellStackValidForGrowingBush);

        }

        private void ApplyGrowthIn(CellStack cellStack)
        {
            if (cellStack == null)
                return;

            if(ivyStateHandler.CanGrowRootIn(cellStack))
            {
                gameplayController.creationUtility.PlaceCellInBoard(ServiceLocator.Find<CellFactory>().CreateIvyRootCell(), cellStack);
                ivyStateHandler.AddToIvyRootsCellStack(cellStack);
                ivyValidCellsUpdater.RemoveFromValidsForGrowingRoots(cellStack);
                presentationHandler.ShowRootGrowIn(cellStack);
            }
            else
            {
                gameplayController.creationUtility.PlaceTileInBoard(ServiceLocator.Find<TileFactory>().CreateIvyBushTile(), cellStack);
                ivyStateHandler.AddIvyBushesCellStack(cellStack);
                ivyValidCellsUpdater.RemoveFromValidsForGrowingBushes(cellStack);
                presentationHandler.ShowBushGrowIn(cellStack);
            }
        }
    }
}