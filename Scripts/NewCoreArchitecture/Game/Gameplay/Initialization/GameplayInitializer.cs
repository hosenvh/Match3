using Match3.Game.Gameplay.Physics;
using Match3.Game.Gameplay.SubSystems.TileRandomPlacement;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Tiles.Explosives;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.TileGeneration;
using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Game.DynamicDifficulty;
using Match3.Game.Gameplay.SubSystems.BuoyantMechanic;
using Match3.Game.Gameplay.SubSystems.DuckMechanic;
using Match3.Profiler;

namespace Match3.Game.Gameplay.Initialization
{
    public class GameplayInitializationFinishedEvent : GameEvent
    {
    }

    // TODO: Chance the nature of extractors to initializers.
    public class GameplayInitializer
    {
        BoardConfig boardConfig;
        GameplayController gpc;
        
        public void Initialize(GameplayController gpc, BoardConfig boardConfig, LifeConsumer lifeConsumer)
        {
            this.gpc = gpc;
            this.boardConfig = boardConfig;
            gpc.Setup(new GameBoardCreator().CreateFrom(boardConfig), lifeConsumer, boardConfig.levelConfig.difficultyLevel);

            InitialTileGenerationData();
            InitialGoals();
            InitialStoppingCondition();
            InitialRainbowValues();
            InitialBoosters();
            InitialReservedBoosterRewards();
            InitialRocketBoxPriorities();
            InitialBuoyantsData();
            InitialDucksData();

            if(boardConfig.levelConfig.enableLemonadeSideFalling)
                gpc.GetSystem<PhysicsSystem>().SetSideFallPriorityFor<Lemonade>();

            ServiceLocator.Find<EventManager>().Propagate(new GameplayInitializationFinishedEvent(), this);
            gpc.Start();
        }

        private void InitialColorChances()
        {
            TileSourceSystem tileSourceSystem = gpc.GetSystem<TileSourceSystem>();

            var colorChances = new ColorChanceExtractor().ExtractGenerationChancesFrom(boardConfig);
            foreach (var entry in colorChances)
                tileSourceSystem.SetColorGenerationChance(entry.Key, entry.Value);

            var dirtinessChances = new ColorChanceExtractor().ExtractDirtinessChancesFrom(boardConfig);
            foreach (var entry in dirtinessChances)
                tileSourceSystem.SetColorDirtinessChance(entry.Key, entry.Value);
        }

        private void InitialTileGenerationData()
        {
            InitialColorChances();

            var tileGenerationLimits =  new TileGenerationInfoExtractor().ExtractFrom(boardConfig);

            var system = gpc.GetSystem<TileSourceSystem>();
            system.SetSpecialTileLimits(tileGenerationLimits);
            system.SetGasCylinderStartCountDonw(boardConfig.generationGasCylinderStartCountdown);

            system.AddGenerationCondition<NutTileSource>(new EmptinessTileSourceGenerationConditinon());
            system.AddGenerationCondition<LemonadeTileSource>(new EmptinessTileSourceGenerationConditinon());
            system.AddGenerationCondition<ColoredBeadTileSource>(new EmptinessTileSourceGenerationConditinon());
            system.AddGenerationCondition<CatColoredBeadTileSource>(new EmptinessTileSourceGenerationConditinon());
            system.AddGenerationCondition<RocketBoxTileSource>(new EmptinessTileSourceGenerationConditinon());
            system.AddGenerationCondition<GasCylinderTileSource>(new EmptinessTileSourceGenerationConditinon());
            system.AddGenerationCondition<ButterflyTileSource>(new ButterflyTileSourceGenerationCondition());
            system.AddGenerationCondition<JamJarTileSource>(new EmptinessTileSourceGenerationConditinon());
            system.AddGenerationCondition<DynamicTileSource>(new EmptinessTileSourceGenerationConditinon());

        }
        private void InitialGoals()
        {
            foreach (var goal in new GoalInfoExtaractor().Extract(boardConfig))
                AddGoal(goal.goalType, goal.goalAmount);
        }


        private void InitialStoppingCondition()
        {
            var stoppingSystem = gpc.GetSystem<LevelStoppingSystem>();
            stoppingSystem.AddStopCondition(new MovementStopCondition(boardConfig.GetLevelMaxMove()));

            // NOTE: This condition is performance heavy. Try add it only when is needed.
            stoppingSystem.AddStopCondition(new GasCylinderStopCondition(gpc.GameBoard()));
        }



        private void AddGoal(GoalTargetType goalType, int goalAmount)
        {
            gpc.GetSystem<LevelStoppingSystem>().AddGoal(goalType, goalAmount);
        }


        private void InitialRainbowValues()
        {
            var balanceManager = ServiceLocator.Find<RainbowFillValueBalanceManager>();
            var values = boardConfig.levelConfig.rainbowAddValues;
            var dynamicDifficultyFactor = ServiceLocator.Find<DynamicDifficultyManager>().dynamicDifficultyApplier.EffectiveRainbowAddValueFactor;
            gpc.GetSystem<RainbowMechanic.RainbowGenerationSystem>().SetExplosiveValue<Rocket>(balanceManager.CurrentFillValueFor(values[0] * dynamicDifficultyFactor));
            gpc.GetSystem<RainbowMechanic.RainbowGenerationSystem>().SetExplosiveValue<Bomb>(balanceManager.CurrentFillValueFor(values[1] * dynamicDifficultyFactor));
            gpc.GetSystem<RainbowMechanic.RainbowGenerationSystem>().SetExplosiveValue<Dynamite>(balanceManager.CurrentFillValueFor(values[2] * dynamicDifficultyFactor));
            gpc.GetSystem<RainbowMechanic.RainbowGenerationSystem>().SetExplosiveValue<TNTBarrel>(balanceManager.CurrentFillValueFor(values[3] * dynamicDifficultyFactor));
        }



        private void InitialBoosters()
        {
            var gameManager = Base.gameManager;
            for (int i = gameManager.profiler.isBoosterSelected.Length - 1; i >= 0; i--)
                if (gameManager.profiler.isBoosterSelected[i] || gameManager.profiler.BoosterManager.IsInfiniteBoosterAvailable(i))
                    InitialBoostersWithIndex(i);
        }

        private void InitialReservedBoosterRewards()
        {
            LevelReservedRewardsHandler profiler = Base.gameManager.levelSessionProfiler.BoostersReservedRewardsHandler;
            List<Type> rewards = profiler.GetReservedRewardTypes();
            foreach (Type reward in rewards)
            {
                InitialBoostersWithIndex(index: RewardConversionUtility.GetBoosterIndexFromRewardType(reward));
                profiler.ConsumeReservedReward(reward);
            }
        }

        private void InitialBoostersWithIndex(int index)
        {
            var tileRandomPlacer = gpc.GetSystem<TileRandomPlacementSystem>().GetTilePlacer<DefaultTileRandomPlacer>();
            switch (index)
            {
                case 2:
                    tileRandomPlacer.AddTilesToPlace<Rainbow>();
                    tileRandomPlacer.AddTilesToPlace<Dynamite>();
                    break;
                case 1:
                    tileRandomPlacer.AddTilesToPlace<Rainbow>();
                    break;
                case 0:
                    tileRandomPlacer.AddTilesToPlace<Bomb>();
                    tileRandomPlacer.AddTilesToPlace<Bomb>();
                    break;
            }
        }
        
        private void InitialRocketBoxPriorities()
        {
            foreach (var priorityData in boardConfig.levelConfig.rocketBoxPriorities)
                gpc.rocketTargetFinder.AddTargetingData(
                    priorityData.priority,
                    priorityData.targetFinder.CreateTargetFinder(),
                    priorityData.gameplayCondition.CreateCondition());
        }


        private void InitialBuoyantsData()
        {
            var generationSystem = gpc.GetSystem<BuoyantGenerationSystem>();

            int inBoardMax = boardConfig.levelConfig.buoyantGenerationData.inBoardMax;

            int inLevelMax = 0;
            var targetGoalType = new SimpleGoalType(typeof(Buoyant));
            foreach (var goal in gpc.GetSystem<LevelStoppingSystem>().AllGoals())
                if (goal.goalType.Is(targetGoalType))
                    inLevelMax = goal.goalAmount;


            generationSystem.SetMaxBouyants(inLevelMax, inBoardMax);

            // TODO: Try move this to the systems.
            if (inLevelMax == 0)
            {
                gpc.RemoveSystem<BuoyantGenerationSystem>();
                gpc.RemoveSystem<BuoyantMovementSystem>();
            }

        }

        private void InitialDucksData ()
        {
            int inBoardMax = boardConfig.levelConfig.duckGenerationData.inBoardMax;
            int inLevelMax = boardConfig.levelConfig.duckGenerationData.inLevelMax;
            DuckSystem.SetMaxDucksCount(inLevelMax, inBoardMax);

            // TODO: Try move this to the systems.
            if (inLevelMax == 0)
            {
                gpc.RemoveSystem<DuckSystem>();
            }
        }
    }
}