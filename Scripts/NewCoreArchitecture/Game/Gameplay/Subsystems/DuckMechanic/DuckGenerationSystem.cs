using System;
using System.Linq;
using System.Collections.Generic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.HoneyExpansion;
using Match3.Game.Gameplay.TileGeneration;
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Cells;

namespace Match3.Game.Gameplay.SubSystems.DuckMechanic
{
    public class GenerationData
    {
        public int waitTime;
        public readonly int toGenerateCount;

        public GenerationData (int waitTime, int toGenerateCount)
        {
            this.waitTime = waitTime;
            this.toGenerateCount = toGenerateCount;
        }
    }

    [After(typeof(DestructionManagement.DestructionSystem))]
    [After(typeof(General.UserMovementManagementSystem))]
    [Before(typeof(DuckMovementSystem))]
    public class DuckGenerationSystem : DuckSystem
    {
        private TileSourceSystem tileSourceSystem;
        private HoneyExpansionActivatingData honeyExpansionActivatingData;

        public DuckGenerationSystem (GameplayController gameplayController) : base (gameplayController)
        { }

        public override void Start()
        {
            tileSourceSystem = gameplayController.GetSystem<TileSourceSystem>();
            honeyExpansionActivatingData = GetFrameData<HoneyExpansionActivatingData>();

            foreach (Duck duck in ducks)
                duck.SetRocketBoxColor(tileSourceSystem.ChooseAColor());

            totalDucksCountInBoard = ducks.Count;
            totalRemainingDucksCountToGenerate = maxDucksCountInLevel - totalDucksCountInBoard;

            if (maxDuckCountInBoard - totalDucksCountInBoard > 0)
                TryScheduleGeneration(GENERATION_DEFAULT_TURNS_TO_WAIT_COUNT, maxDuckCountInBoard - totalDucksCountInBoard);

            CheckForHoneyExpansionActivatingDuck();
        }

        public override void Update (float dt)
        {
            if (IsUserMoved())
                ProcessScheduleGenerations();

            UpdateDucksForGenerate();
            CheckForHoneyExpansionActivatingDuck();
        }

        private void ProcessScheduleGenerations ()
        {
            for (int i = scheduledGenerationData.Count - 1; i >= 0; --i)
            {
                GenerationData data = scheduledGenerationData[i];
                data.waitTime--;

                if (data.waitTime <= 0)
                {
                    scheduledGenerationData.RemoveAt(i);
                    GenerateDucksFor(data);
                }
            }
        }

        private void GenerateDucksFor (GenerationData data)
        {
            var availableRiverCells = ExtractAvailableRiverCells(IsRiverAvailableForGeneration);
            availableRiverCells.Shuffle();

            int totalCreated = 0;
            for (int i = 0; i < data.toGenerateCount && i < availableRiverCells.Count && i < totalRemainingDucksCountToGenerate; ++i)
            {
                totalCreated++;
                var duck = tileFactory.CreateDuckTile(childTilesTypes);
                duck.As<Duck>().SetRocketBoxColor(tileSourceSystem.ChooseAColor());
                gameplayController.creationUtility.PlaceTileInBoard(duck, availableRiverCells[i].Parent());
            }

            totalDucksCountInBoard += totalCreated;
            totalRemainingDucksCountToGenerate -= totalCreated;

            if (totalCreated < data.toGenerateCount)
                TryScheduleGeneration(0, data.toGenerateCount - totalCreated);
        }

        private bool IsRiverAvailableForGeneration(RiverCell riverCell)
        {
            var riverCellStack = riverCell.Parent();
            return
                QueryUtilities.IsFullyFree(riverCellStack) &&
                QueryUtilities.IsGoingToBeHit(riverCellStack, cellStackBoard) == false &&
                QueryUtilities.HasAnyTile(riverCellStack) == false;
        }

        private void UpdateDucksForGenerate ()
        {
            foreach (var tile in GetFrameData<GeneratedObjectsData>().tiles)
            {
                if (tile is Duck duck)
                    ducks.Add(duck);
            }
        }

        private void CheckForHoneyExpansionActivatingDuck()
        {
            Duck duck = ducks.FirstOrDefault();
            if (duck == null)
                return;
            
            if (HasDuckTileWithHoneyJarItem())
                honeyExpansionActivatingData.areThereAnyActivatingItems = true;

            bool HasDuckTileWithHoneyJarItem() => duck.GetChildItems().Any(child => child.Item == typeof(HoneyJar));
        }
    }
}