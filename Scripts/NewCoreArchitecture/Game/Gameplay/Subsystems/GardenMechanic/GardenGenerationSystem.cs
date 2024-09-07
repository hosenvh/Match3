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
using System.Linq;
using System.Collections.Generic;
using Match3.Game.Gameplay.Matching;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.GardenMechanic
{

    public struct GardenGenerationKeyType : KeyType
    {}

    public interface GardenGenerationPresentationHandler : PresentationHandler
    {
        void PlaceGeneratedTiles(CellStack origin, List<CellStack> targets, Action<int> onTargetReached, Action onCompleted);
    }

    public class GardenGenerationSystem : GameplaySystem
    {
        const int GenerationAmountPerGarden = 4;

        List<Garden> gardens = new List<Garden>();

        HashSet<int> toGenerateIndexes = new HashSet<int>();
        HashSet<int> beingGeneratedIndexes = new HashSet<int>();

        RandomCellStackChooser randomCellStackChooser;
        GardenGenerationPresentationHandler presentationHandler;
        TileFactory tileFactory;

        public GardenGenerationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            foreach(var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if(HasTile<Garden>(cellStack))
                    gardens.Add(FindTile<Garden>(cellStack));

            randomCellStackChooser = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard());
            presentationHandler = gameplayController.GetPresentationHandler<GardenGenerationPresentationHandler>();
            tileFactory = ServiceLocator.Find<TileFactory>();
        }

        public override void Update(float dt)
        {
            if (toGenerateIndexes.Count > 0)
                GetSessionData<InputControlData>().AddLockedBy<GardenGenerationKeyType>();


            if(toGenerateIndexes.Count > 0 && GetFrameData<StabilityData>().wasStableLastChecked)
                StartGeneration();

            UpdateGardenStates();
        }

        private void StartGeneration()
        {
            //GetSessionData<StabilityControlData>().shouldForceStablize = true;
            beingGeneratedIndexes.AddRange(toGenerateIndexes);
            toGenerateIndexes.Clear();

            var chosenCells = randomCellStackChooser.Choose(
                GenerationAmountPerGarden * beingGeneratedIndexes.Count,
                IsValid);

            foreach (var cell in chosenCells)
                cell.CurrentTileStack().GetComponent<LockState>().LockBy<GardenGenerationKeyType>();

            var beingGeneratedIndexesCopy = new HashSet<int>(beingGeneratedIndexes);
            foreach (var index in beingGeneratedIndexesCopy)
            {
                gardens[index].Reset();

                var targetList = chosenCells.GetRange(0, UnityEngine.Mathf.Min(GenerationAmountPerGarden, chosenCells.Count));
                chosenCells.RemoveRange(0, UnityEngine.Mathf.Min(GenerationAmountPerGarden, chosenCells.Count));

                

                var indexCapture = index;

                if (targetList.Count > 0)
                    presentationHandler.PlaceGeneratedTiles(
                        gardens[index].Parent().Parent(),
                        targetList,
                        (i) => CreateTileIn(targetList[i]),
                        () => RemoveBeingGenerated(indexCapture));
                else
                    RemoveBeingGenerated(indexCapture);

            }
        }

        void RemoveBeingGenerated(int gardenIndex)
        {
            beingGeneratedIndexes.Remove(gardenIndex);
            if (beingGeneratedIndexes.Count == 0)
                GetSessionData<InputControlData>().RemoveLockedBy<GardenGenerationKeyType>();
        }

        private void CreateTileIn(CellStack cellStack)
        {
            var tileStack = tileFactory.CreateTileStack();
            tileStack.Push(tileFactory.CreateCleanColoredBeadTile(TileColor.Pink));

            if (cellStack.HasTileStack())
            {
                cellStack.CurrentTileStack().Destroy();
            }

            cellStack.SetCurrnetTileStack(tileStack);
            tileStack.SetPosition(cellStack.Position());

            ServiceLocator.Find<EventManager>().Propagate(new TileStackGeneratedEvent(tileStack), this);
        }

        public  bool IsValid(CellStack cellStack)
        {
            return
                cellStack.HasTileStack()
                && cellStack.CurrentTileStack().IsDepleted() == false
                && cellStack.CurrentTileStack().Top() is ColoredBead
                && (cellStack.CurrentTileStack().Top() as ColoredBead).GetComponent<TileColorComponent>().color != TileColor.Pink;
        }

        private void UpdateGardenStates()
        {
            for (int i = 0; i < gardens.Count; ++i)
            {
                var garden = gardens[i];

                if (garden.IsFilled() && IsInProcess(i) == false)
                    toGenerateIndexes.Add(i);
            }
        }

        private bool IsInProcess(int i)
        {
            return toGenerateIndexes.Contains(i) || beingGeneratedIndexes.Contains(i);
        }
    }

}