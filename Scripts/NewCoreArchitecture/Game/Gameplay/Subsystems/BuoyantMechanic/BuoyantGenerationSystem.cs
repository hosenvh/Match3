using KitchenParadise.Utiltiy.Base;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.TileGeneration;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.BuoyantMechanic
{

    [After(typeof(DestructionManagement.DestructionSystem))]
    [After(typeof(General.UserMovementManagementSystem))]
    [Before(typeof(BuoyantMovementSystem))]
    public class BuoyantGenerationSystem : BuoyantSystem
    {
        const int DEFAULT_GENERATION_DURATION = 2;

        class GenerationData
        {
            public int waitTime;
            public int generationCount;

            public GenerationData(int waitTime, int generationCount)
            {
                this.waitTime = waitTime;
                this.generationCount = generationCount;
            }
        }

        int inLevelMaxBuoyants;
        int inBoardMaxBuoyants;

        int totalRemainingBouyantsToGenerate;
        int totalBouyantsInBoard;

        List<GenerationData> scheduledGenerationsData = new List<GenerationData>(4);

        TileFactory tileFactory;
        TileSourceSystem tileSourceSystem;

        public BuoyantGenerationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            tileFactory = ServiceLocator.Find<TileFactory>();
        }

        public void SetMaxBouyants(int levelMax, int boardMax)
        {
            inLevelMaxBuoyants = levelMax;
            inBoardMaxBuoyants = boardMax;
        }

        public override void Start()
        {
            tileSourceSystem = gameplayController.GetSystem<TileSourceSystem>();

            totalBouyantsInBoard = 0;
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (QueryUtilities.HasTileOnTop<Buoyant>(cellStack))
                    totalBouyantsInBoard++;

            totalRemainingBouyantsToGenerate = inLevelMaxBuoyants - totalBouyantsInBoard;

            if(inBoardMaxBuoyants - totalBouyantsInBoard > 0)
                TryScheduleGeneration(DEFAULT_GENERATION_DURATION, inBoardMaxBuoyants - totalBouyantsInBoard);
        }

        public override void Update(float dt)
        {
            UpdageBuoyantsInBoardForDestroyedBuoyants();

            if (IsUserMoved())
                ProcessScheduleGenerations();
        }


        private void UpdageBuoyantsInBoardForDestroyedBuoyants()
        {
            int totalDestroyed = 0;
            foreach (var tile in destrotedObjectsData.tiles)
                if (tile is Buoyant)
                    ++totalDestroyed;

            totalBouyantsInBoard -= totalDestroyed;

            if (totalDestroyed > 0)
                TryScheduleGeneration(DEFAULT_GENERATION_DURATION, totalDestroyed);
        }

        private void ProcessScheduleGenerations()
        {
            for (int i = scheduledGenerationsData.Count-1; i >= 0 ; --i)
            {
                GenerationData data = scheduledGenerationsData[i];
                --data.waitTime;
                
                if(data.waitTime<=0)
                {
                    scheduledGenerationsData.RemoveAt(i);
                    GenerateBuoyantsFor(data);
                }
            }
        }

        private void GenerateBuoyantsFor(GenerationData data)
        {
            var availableRiverCells = ExtractAvailableRiverCells(IsRiverAvailableForGeneration);
            availableRiverCells.Shuffle();

            int totalCreated = 0;
            for (int i = 0; i < data.generationCount && i < availableRiverCells.Count && i < totalRemainingBouyantsToGenerate; ++i)
            {
                ++totalCreated;
                var buoyant = tileFactory.CreateBuoyantTile(tileSourceSystem.ChooseAColor());
                gameplayController.creationUtility.PlaceTileInBoard(buoyant, availableRiverCells[i].Parent());
            }

            totalBouyantsInBoard += totalCreated;
            totalRemainingBouyantsToGenerate -= totalCreated;

            if (totalCreated < data.generationCount)
                TryScheduleGeneration(0, data.generationCount - totalCreated);
        }

        private void TryScheduleGeneration(int waitTime, int generationCount)
        {
            if (totalBouyantsInBoard >= inBoardMaxBuoyants || totalRemainingBouyantsToGenerate <= 0)
                return;

            var avaiableGenerations = Math.Min(inBoardMaxBuoyants - totalBouyantsInBoard, totalRemainingBouyantsToGenerate);
            generationCount = Math.Min(generationCount, avaiableGenerations);

            scheduledGenerationsData.Add(new GenerationData(waitTime, generationCount));
        }


        private bool IsRiverAvailableForGeneration(RiverCell riverCell)
        {
            var riverCellStack = riverCell.Parent();
            return
                QueryUtilities.IsFullyFree(riverCellStack) &&
                QueryUtilities.IsGoingToBeHit(riverCellStack, cellStackBoard) == false &&
                QueryUtilities.HasAnyTile(riverCellStack) == false;
        }

    }
}