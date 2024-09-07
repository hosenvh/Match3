using KitchenParadise.Utiltiy.Base;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Match3.Game.Gameplay.QueryUtilities;
using static Match3.Game.Gameplay.ActionUtilites;

namespace Match3.Game.Gameplay.SubSystems.ChickenNestMechanic
{
    public struct ChickenNestGenerationKeyType : KeyType
    {
    }

    public interface ChickenNestPresentationHandler : PresentationHandler
    {
        void PlaceGeneratedTilesIn(CellStack origin, List<CellStack> targets, Action<int> onTargetReached, Action onCompleted);
        void StartRemoving(ChickenNest chickenNest);
    }

    [After(typeof(HitManagement.HitApplyingSystem))]
    public class ChickenNestSystem : GameplaySystem
    {
        private const int GenerationAmountPerChickenNest = 3;

        private readonly List<ChickenNest> chickenNests = new List<ChickenNest>();

        private readonly HashSet<ChickenNest> readyForGenerationChickenNests = new HashSet<ChickenNest>();
        private readonly HashSet<ChickenNest> generatingChickenNests = new HashSet<ChickenNest>();

        private RandomCellStackChooser randomCellStackChooser;
        private ChickenNestPresentationHandler presentationHandler;
        private TileFactory tileFactory;

        public ChickenNestSystem(GameplayController gameplayController) : base(gameplayController)
        {
            SetupFields();
            FetchChickenNestTilesFromBoard();
        }

        private void SetupFields()
        {
            randomCellStackChooser = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard());
            presentationHandler = gameplayController.GetPresentationHandler<ChickenNestPresentationHandler>();
            tileFactory = ServiceLocator.Find<TileFactory>();
        }

        private void FetchChickenNestTilesFromBoard()
        {
            foreach (CellStack cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTile<ChickenNest>(cellStack))
                    chickenNests.Add(FindTile<ChickenNest>(cellStack));
        }

        public override void Update(float deltaTime)
        {
            TryToFindReadyToGenerateChickenNests();
            TryToStartGenerationForReadyToGenerateChickenNests();

            UpdateInputControlDataLockState();
            TryToDeactivateSystem();
        }

        private void UpdateInputControlDataLockState()
        {
            if (ShouldLockInputControlData())
                GetSessionData<InputControlData>().AddLockedBy<ChickenNestGenerationKeyType>();
            else
                GetSessionData<InputControlData>().RemoveLockedBy<ChickenNestGenerationKeyType>();
        }

        private bool ShouldLockInputControlData()
        {
            return readyForGenerationChickenNests.Count > 0 || generatingChickenNests.Count > 0;
        }

        private void TryToFindReadyToGenerateChickenNests()
        {
            foreach (ChickenNest chickenNest in chickenNests)
                if (chickenNest.ShouldGetReadyForGeneration())
                    readyForGenerationChickenNests.Add(chickenNest);

            foreach (ChickenNest readyForGenerationChickenNest in readyForGenerationChickenNests)
                chickenNests.Remove(readyForGenerationChickenNest);
        }

        private void TryToStartGenerationForReadyToGenerateChickenNests()
        {
            if (CanStartGenerationForReadyToGenerateChickenNests())
                StartGenerationForReadyToGenerateChickenNests();
        }

        private bool CanStartGenerationForReadyToGenerateChickenNests()
        {
            return readyForGenerationChickenNests.Count > 0 && GetFrameData<StabilityData>().wasStableLastChecked;
        }

        private void StartGenerationForReadyToGenerateChickenNests()
        {
            MoveAllReadyToGenerateChickenNestsToGeneratingChickenNestsList();

            List<CellStack> targetCells = ChooseGeneratingChickenNestsTargets(neededTargetsCount: GenerationAmountPerChickenNest * generatingChickenNests.Count);
            LockCellStacksAsChickenNestGenerationTargets(targetCells);

            HashSet<ChickenNest> generatingChickenNestsCopy = new HashSet<ChickenNest>(generatingChickenNests);
            foreach (ChickenNest chickenNest in generatingChickenNestsCopy)
            {
                List<CellStack> targetCellsForThisChickenNest = CutListElements(from: targetCells, neededCount: GenerationAmountPerChickenNest);
                StartGenerationForChickenNest(chickenNest, targetCellsForThisChickenNest);
            }
        }

        private void MoveAllReadyToGenerateChickenNestsToGeneratingChickenNestsList()
        {
            generatingChickenNests.AddRange(readyForGenerationChickenNests);
            readyForGenerationChickenNests.Clear();
        }

        private List<CellStack> ChooseGeneratingChickenNestsTargets(int neededTargetsCount)
        {
            return randomCellStackChooser.Choose(neededTargetsCount, validator: IsValidToChooseAsChickenNestGenerationTargetCellStack);
        }

        private bool IsValidToChooseAsChickenNestGenerationTargetCellStack(CellStack cellStack)
        {
            return
                cellStack.HasTileStack()
             && cellStack.CurrentTileStack().IsDepleted() == false
             && cellStack.CurrentTileStack().Top() is ColoredBead;
        }

        private void LockCellStacksAsChickenNestGenerationTargets(List<CellStack> cells)
        {
            foreach (CellStack cell in cells)
                Lock<ChickenNestGenerationKeyType>(cell.CurrentTileStack());
        }

        private void StartGenerationForChickenNest(ChickenNest chickenNest, List<CellStack> targetCellsForThisChickenNest)
        {
            FullyLock<ChickenNestGenerationKeyType>(chickenNest.Parent().Parent());
            if (targetCellsForThisChickenNest.Count > 0)
                presentationHandler.PlaceGeneratedTilesIn(
                      origin: chickenNest.Parent().Parent(),
                      targets: targetCellsForThisChickenNest,
                      onTargetReached: i => CreateTileIn(targetCellsForThisChickenNest[i]),
                      onCompleted: () => DestroyGeneratingChickenNest(chickenNest));
            else
                DestroyGeneratingChickenNest(chickenNest);
            presentationHandler.StartRemoving(chickenNest);
        }

        private void CreateTileIn(CellStack cellStack)
        {
            gameplayController.creationUtility.ReplaceTileInBoard(tileFactory.CreateChickTile(), cellStack);
        }

        private void DestroyGeneratingChickenNest(ChickenNest chickenNest)
        {
            FullyUnlock(chickenNest.Parent().Parent());

            chickenNest.MarkAsDestroyed();
            FullyDestroy(chickenNest);
            GetFrameData<DestructionData>().tilesToDestroy.Add(chickenNest);

            generatingChickenNests.Remove(chickenNest);
        }

        private void TryToDeactivateSystem()
        {
            if (ShouldDeactivateSystem())
                gameplayController.DeactivateSystem<ChickenNestSystem>();
        }

        private bool ShouldDeactivateSystem()
        {
            return chickenNests.Count == 0 && readyForGenerationChickenNests.Count == 0 && generatingChickenNests.Count == 0;
        }


        private List<T> CutListElements<T>(List<T> from, int neededCount)
        {
            List<T> result = from.GetRange(0, Mathf.Min(neededCount, from.Count));
            from.RemoveRange(0, result.Count);
            return result;
        }
    }
}