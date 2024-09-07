using Match3.Game.Gameplay.Core;
using System.Linq;
using System.Collections.Generic;
using Match3.Game.Gameplay.Input;
using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.Swapping;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.Physics;
using System;
using Match3.Game.Gameplay.ExplosionManagement;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.SubSystems.Stability;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.SubSystems.ArtifactMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ArtifactMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.PowerUpManagement;
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ExplosionManagement;
using Match3.Game.Gameplay.SubSystemsData.FrameData.HoneyExpansion;
using Match3.Game.Gameplay.SubSystems.RiverMechanic;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystems.TileRandomPlacement;
using Match3.Game.Gameplay.SubSystems.ExtraMoveMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.LevelEnding;
using Match3.Game.Gameplay.SubSystems.GardenMechanic;
using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;
using Match3.Game.Gameplay.SubSystemsData.SessionData.General;
using Match3.Game.Gameplay.SubSystems.HoneyMechanic;
using Match3.Game.Gameplay.SubSystems.WinSequence;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.MatchingCombo;
using Match3.Game.Gameplay.SubSystems.ButterflyMechanic;
using Match3.Game.Gameplay.TileGeneration;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RocketMechanic;
using Match3.Game.Gameplay.SubSystems.VacuumCleanerMechanic;
using Match3.Game.Gameplay.SubSystems.PadlockMechanic;
using Match3.Game.Gameplay.SubSystems.IvyMechanic;
using Match3.Game.Gameplay.SubSystems.GasCylinderMechanic;
using Match3.Game.Gameplay.BeachBallMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.BeachBallMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RiverMechanic;
using Match3.Game.Gameplay.SubSystems.TableClothMechanic;
using Match3.Game.Gameplay.SubSystems.LilyPadBudMechanic;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;
using Match3.Game.Gameplay.SubSystems.BuoyantMechanic;
using Match3.Game.Gameplay.SubSystems.DuckMechanic;
using Match3.Game.Gameplay.SubSystems.GrassSackMechanic;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.CatMechanic;
using Match3.Game.Gameplay.SubSystems.ChickenNestMechanic;
using Match3.Game.Gameplay.SubSystems.IceMakerMechanic;
using Match3.Game.Gameplay.SubSystems.RocketBoxMechanic;
using Match3.Game.Gameplay.SubSystems.BalloonMechanic;
using Match3.Game.Gameplay.Subsystems.Compass;
using Match3.Game.Gameplay.Subsystems.IvySack;
using Match3.Game.Gameplay.SubSystems.LouieMechanic;
using Match3.Presentation.Gameplay;
using Match3.Utility.GolmoradLogging;


namespace Match3.Game.Gameplay
{

    public class LevelEndedEvent : GameEvent
    {
        public readonly LevelResult result;

        public LevelEndedEvent(LevelResult result)
        {
            this.result = result;
        }
    }

    public class LevelScoredEvent : GameEvent
    {
        public int score;

        public LevelScoredEvent(int score)
        {
            this.score = score;
        }
    }

    public class LevelStartedEvent : GameEvent
    {
        public readonly GameplayState currentGameplayState;

        public LevelStartedEvent(GameplayState currentGameplayState)
        {
            this.currentGameplayState = currentGameplayState;
        }
    }

    public class LevelStartResourceConsumingEvent : GameEvent
    {

    }

    public class LevelStartedWithBoosters : GameEvent
    {
        public bool HasDoubleBomb { get; private set; }
        public bool HasRainbow { get; private set; }
        public bool HasTntRainbow { get; private set; }

        public LevelStartedWithBoosters(ConsumedBoostersState consumedBoostersState)
        {
            HasDoubleBomb = consumedBoostersState.hasConsumedDoubleBombBooster;
            HasRainbow = consumedBoostersState.hasConsumedRainbowBooster;
            HasTntRainbow = consumedBoostersState.hasConsumedTntRainbowBooster;
        }
    }

    public interface PresentationHandler
    { }

    public interface BlackBoardData : Component
    {
        void Clear();
    }

    public class InterSystemBlackBoard : BasicSpecializedEntity<BlackBoardData>
    {
        public void Clear()
        {
            foreach (var data in AllComponents())
                data.Clear();
        }
    }

    public enum GameplaySystemTag { General, EndOnly, StartOnly }
    public interface IGameplayController
    {
        GameBoard GameBoard();
        void AddPresentationHandler(PresentationHandler handler);

        T GetPresentationHandler<T>() where T : PresentationHandler;
        T GetSystem<T>() where T : IGameplaySystem;

        bool IsDeactive<T>() where T : IGameplaySystem;

        void Update(float dt);


        InterSystemBlackBoard FrameCenteredBlackBoard();

        InterSystemBlackBoard SessionCenteredBlackBoard();
    }

    public interface LifeConsumer
    {
        void ConsumeLife();
    }

    public class GameplayController : IGameplayController
    {
 
        GameBoard gameBoard;

        Dictionary<GameplaySystemTag, HashSet<GameplaySystem>> systemTags;
        Dictionary<GameplaySystem, int> systemOrderings = new Dictionary<GameplaySystem, int>();

        List<GameplaySystem> gameplaySystems = new List<GameplaySystem>();

        List<GameplaySystem> activeSystems = new List<GameplaySystem>();



        List<GameplaySystem> gameplaySystemsToDeactivate = new List<GameplaySystem>();
        List<GameplaySystem> gameplaySystemsToActivate = new List<GameplaySystem>();

        InterSystemBlackBoard frameCenteredSystemBlackBoard = new InterSystemBlackBoard();
        InterSystemBlackBoard sessionCenteredSystemBlackBoard = new InterSystemBlackBoard();


        public BoardBlockageController boardBlockageController;
        public MatchingRulesTable matchingRulesTable;
        public BoardStabilityCalculator boardStabilityCalculator;
        public CreationController creationUtility;
        public RocketTargetFinder rocketTargetFinder;

        List<PresentationHandler> presentationHandlers = new List<PresentationHandler>();

        private bool isInitialized = false;
        

        // TODO: Move the data assignment to separate entity.
        public void Setup(GameBoard gameBoard, LifeConsumer lifeConsumer, DifficultyLevel currentDifficulty)
        {

            var cellFactory = ServiceLocator.Find<CellFactory>();
            var tileFactory = ServiceLocator.Find<TileFactory>();

            this.gameBoard = gameBoard;

            systemTags = new Dictionary<GameplaySystemTag, HashSet<GameplaySystem>>();

            systemTags[GameplaySystemTag.EndOnly] = new HashSet<GameplaySystem>();
            systemTags[GameplaySystemTag.StartOnly] = new HashSet<GameplaySystem>();
            systemTags[GameplaySystemTag.General] = new HashSet<GameplaySystem>();

            boardBlockageController = new BoardBlockageController(gameBoard);
            matchingRulesTable = new MatchingRulesTable();
            boardStabilityCalculator = new BoardStabilityCalculator(gameBoard);


            frameCenteredSystemBlackBoard.AddComponent(new ExecutedSwapsData());
            frameCenteredSystemBlackBoard.AddComponent(new RequestedSwapsData());
            frameCenteredSystemBlackBoard.AddComponent(new CreatedMatchesData());
            frameCenteredSystemBlackBoard.AddComponent(new DestroyedObjectsData());
            frameCenteredSystemBlackBoard.AddComponent(new RainbowActivationData());
            frameCenteredSystemBlackBoard.AddComponent(new PowerUpActivationData());
            frameCenteredSystemBlackBoard.AddComponent(new ExplosionActivationData());
            frameCenteredSystemBlackBoard.AddComponent(new GeneratedHitsData());
            frameCenteredSystemBlackBoard.AddComponent(new AppliedHitsData());
            frameCenteredSystemBlackBoard.AddComponent(new DestructionData());
            frameCenteredSystemBlackBoard.AddComponent(new UserRequestedTileStackActivationData());
            frameCenteredSystemBlackBoard.AddComponent(new GeneratedTileStacksData());
            frameCenteredSystemBlackBoard.AddComponent(new GeneratedObjectsData());
            frameCenteredSystemBlackBoard.AddComponent(new SuccessfulUserActivationData());
            frameCenteredSystemBlackBoard.AddComponent(new SuccessfulSwapsData());
            frameCenteredSystemBlackBoard.AddComponent(new StabilityData());
            frameCenteredSystemBlackBoard.AddComponent(new ArtifactHitData());
            frameCenteredSystemBlackBoard.AddComponent(new PowerUpRequestData());
            frameCenteredSystemBlackBoard.AddComponent(new DirectRainbowActivationRequestData());
            frameCenteredSystemBlackBoard.AddComponent(new InternalExplosionActivationData());
            frameCenteredSystemBlackBoard.AddComponent(new UserMovementData());
            frameCenteredSystemBlackBoard.AddComponent(new MovesToAddData());
            frameCenteredSystemBlackBoard.AddComponent(new LevelWinningData());
            frameCenteredSystemBlackBoard.AddComponent(new RocketHitData());
            frameCenteredSystemBlackBoard.AddComponent(new VacuumCleanerHitData());
            frameCenteredSystemBlackBoard.AddComponent(new PadlockHitData());
            frameCenteredSystemBlackBoard.AddComponent(new BeachBallActivationData());
            frameCenteredSystemBlackBoard.AddComponent(new GeneralHitData());
            frameCenteredSystemBlackBoard.AddComponent(new HoneyExpansionActivatingData());
            frameCenteredSystemBlackBoard.AddComponent(new RiverBlockingData());



            sessionCenteredSystemBlackBoard.AddComponent(new StabilityControlData());
            sessionCenteredSystemBlackBoard.AddComponent(new InputControlData());
            sessionCenteredSystemBlackBoard.AddComponent(new PossibleMovesData());
            sessionCenteredSystemBlackBoard.AddComponent(new HoneyGenerationData());
            sessionCenteredSystemBlackBoard.AddComponent(new CompassBoardData());


            creationUtility = new CreationController(frameCenteredSystemBlackBoard.GetComponent<GeneratedObjectsData>(), tileFactory,cellFactory, gameBoard.CellStackBoard());
            rocketTargetFinder = new RocketTargetFinder(this);

            // WARNING: The ordering of the system are extremely important. 
            AddSystem(new StabilizationDetectionSystem(this), GameplaySystemTag.General);
            AddSystem(new ExplosionActivationGatheringSystem(this), GameplaySystemTag.General);
            AddSystem(new RainbowActivationGatheringSystem(this), GameplaySystemTag.General);
            AddSystem(new LevelWinSequenceSystem(this), GameplaySystemTag.EndOnly);
            AddSystem(new HintingSystem(this), GameplaySystemTag.General);
            AddSystem(new TileRandomPlacementSystem(this), GameplaySystemTag.StartOnly);
            AddSystem(new InputSystem(this), GameplaySystemTag.General);
            AddSystem(new SwapExecutionSystem(this), GameplaySystemTag.General);
            AddSystem(new IceMakerSystem(this), GameplaySystemTag.General);
            AddSystem(new MatchDetectionSystem(this), GameplaySystemTag.General);
            AddSystem(new ColoredBeadDirtyingSystem(this), GameplaySystemTag.General);
            AddSystem(new SwapRestorationSystem(this), GameplaySystemTag.General);
            AddSystem(new PowerUpActivationSystem(this), GameplaySystemTag.General);
            AddSystem(new RainbowActivationSystem(this), GameplaySystemTag.General);
            AddSystem(new BeachBallActivationSystem(this), GameplaySystemTag.General);
            AddSystem(new ExplosionActivationSystem(this), GameplaySystemTag.General);
            AddSystem(new ArtifactDiscoverySystem(this), GameplaySystemTag.General);
            AddSystem(new GeneralHitActivatingSystem(this), GameplaySystemTag.General);
            AddSystem(new RocketHitHandlingSystem(this), GameplaySystemTag.General);
            AddSystem(new VacuumCleanerActivationSystem(this), GameplaySystemTag.General);
            AddSystem(new TableClothRemovingSystem(this), GameplaySystemTag.General);
            AddSystem(new PadlockRemovingSystem(this), GameplaySystemTag.General);
            AddSystem(new BalloonsSystem(this), GameplaySystemTag.General);
            AddSystem(new HitGenerationSystem(this), GameplaySystemTag.General);
            AddSystem(new HitApplyingSystem(this), GameplaySystemTag.General);
            AddSystem(new ChickenNestSystem(this), GameplaySystemTag.General);
            AddSystem(new PadlockUnlockingDetectionSystem(this), GameplaySystemTag.General);
            AddSystem(new RocketBoxActivationSystem(this, rocketTargetFinder), GameplaySystemTag.General);
            AddSystem(new ExplosionCreationSystem(this), GameplaySystemTag.General);
            AddSystem(new LemonadeSystem(this), GameplaySystemTag.General);
            AddSystem(new CatManagementSystem(this), GameplaySystemTag.General);
            AddSystem(new LouieSystem(this), GameplaySystemTag.General);
            AddSystem(new CompassDestructionSystem(this),GameplaySystemTag.General);
            AddSystem(new DestructionSystem(this), GameplaySystemTag.General);
            AddSystem(new IvySackSystem(this),GameplaySystemTag.General);
            AddSystem(new RopeBlockageUpdaterSystem(this), GameplaySystemTag.General);
            AddSystem(new HoneyJarSpillingSystem(this), GameplaySystemTag.General);
            AddSystem(new GrassSackOpeningSystem(this, cellFactory, creationUtility), GameplaySystemTag.General);
            AddSystem(new RainbowGenerationSystem(this), GameplaySystemTag.General);
            AddSystem(new GardenGenerationSystem(this), GameplaySystemTag.General);
            AddSystem(new UserMovementManagementSystem(this), GameplaySystemTag.General);
            AddSystem(new CompassRotationSystem(this),GameplaySystemTag.General);
            AddSystem(new LilyPadBudGrowthSystem(this), GameplaySystemTag.General);
            AddSystem(new BuoyantGenerationSystem(this), GameplaySystemTag.General);
            AddSystem(new ButterflyBoardMovementStateDeterminer(this), GameplaySystemTag.General);
            AddSystem(new BuoyantMovementSystem(this), GameplaySystemTag.General);
            AddSystem(new TileSourceSystem(this), GameplaySystemTag.General);
            AddSystem(new ExtraMoveApplyingSystem(this), GameplaySystemTag.General);
            AddSystem(new PhysicsSystem(this), GameplaySystemTag.General);
            AddSystem(new RiverBlockageDetectionSystem(this), GameplaySystemTag.General);
            AddSystem(new RiverMovementSystem(this), GameplaySystemTag.General);
            AddSystem(new DuckDestructionSystem(this), GameplaySystemTag.General);
            AddSystem(new DuckMovementSystem(this), GameplaySystemTag.General);
            AddSystem(new DuckGenerationSystem(this), GameplaySystemTag.General);
            AddSystem(new HoneyExpansionSystem(this), GameplaySystemTag.General);
            AddSystem(new IvyExpansionSystem(this), GameplaySystemTag.General);
            AddSystem(new ArtifactWithRocketActivationSystem(this, rocketTargetFinder), GameplaySystemTag.General);
            AddSystem(new GasCylinderCountdownSystem(this), GameplaySystemTag.General);
            AddSystem(new LevelStoppingSystem(this), GameplaySystemTag.General);
            AddSystem(new ButterflyBoardMovementSystem(this), GameplaySystemTag.General);
            AddSystem(new BoardShufflingSystem(this), GameplaySystemTag.General);
            AddSystem(new LevelFinalScoringSystem(this, currentDifficulty), GameplaySystemTag.EndOnly);
            AddSystem(new LevelEndingSystem(this), GameplaySystemTag.EndOnly);
            AddSystem(new LevelStartResourceConsumingSystem(this, lifeConsumer), GameplaySystemTag.StartOnly);
            AddSystem(new MatchingComboSystem(this), GameplaySystemTag.General);
            AddSystem(new LevelContinuingSystem(this), GameplaySystemTag.General);



            StoreSystemsOrderings();


            ActivateSystemOfTag(GameplaySystemTag.General);
            ActivateSystemOfTag(GameplaySystemTag.StartOnly);

            ResetSystems();

            isInitialized = true;
        }

        void AddSystem(GameplaySystem system, GameplaySystemTag tag)
        {
            gameplaySystems.Add(system);
            systemTags[tag].Add(system);
        }


        public void AddGeneralSystem(GameplaySystem addingSystem, Type afterTo, Type beforeTo)
        {
            if (!isInitialized)
            {
                throw new Exception("Outer adding system only works after GameplayController initialization");
            }
            
            for (int i = 0; i < gameplaySystems.Count; ++i)
            {
                if (gameplaySystems[i].GetType() == afterTo)
                {
                    gameplaySystems.Insert(i + 1, addingSystem);
                    systemTags[GameplaySystemTag.General].Add(addingSystem);
                    StoreSystemsOrderings();
                    ActivateSystem(addingSystem);
                }
            }
        }
        
        
        void ActivateSystemOfTag(GameplaySystemTag tag)
        {
            gameplaySystemsToActivate.AddRange(systemTags[tag]);
        }


        private void StoreSystemsOrderings()
        {
            for (int i = 0; i < gameplaySystems.Count; ++i)
                systemOrderings[gameplaySystems[i]] = i;
        }

        public void StopGame()
        {
            DeactivateSystem<InputSystem>();
            DeactivateSystem<HintingSystem>();
            DeactivateSystem<BoardShufflingSystem>();
            DeactivateSystem<ExtraMoveApplyingSystem>();
            DeactivateSystem<HoneyJarSpillingSystem>();

            ActivateSystem<LevelEndingSystem>();
            ActivateSystem<LevelFinalScoringSystem>(); 
        }

        public void ResetGame()
        {
            sessionCenteredSystemBlackBoard.Clear();
            ActivateSystemOfTag(GameplaySystemTag.General);
            ResetSystems();
        }

        public void ActivateWinSequenceSystems()
        {
            ActivateSystem<LevelWinSequenceSystem>();
            ActivateSystem<StabilizationDetectionSystem>();
            ActivateSystem<MatchDetectionSystem>();
            ActivateSystem<RainbowActivationSystem>();
            ActivateSystem<ExplosionActivationSystem>();
            ActivateSystem<ExplosionCreationSystem>();
            ActivateSystem<HitGenerationSystem>();
            ActivateSystem<HitApplyingSystem>();
            ActivateSystem<DestructionSystem>();
            ActivateSystem<TileSourceSystem>();
            ActivateSystem<PhysicsSystem>();
            ActivateSystem<LevelFinalScoringSystem>();
            ActivateSystem<LevelEndingSystem>();
            ActivateSystem<MatchingComboSystem>();
            ActivateSystem<RocketBoxActivationSystem>();
            ActivateSystem<GeneralHitActivatingSystem>();
            ActivateSystem<RocketHitHandlingSystem>();
        }
        
        public void Start()
        {
            DeactivateRequestedSystems();
            ActivateRequestedSystems();
            foreach (var system in gameplaySystems)
                system.Start();

            DeactivateRequestedSystems();
            ActivateRequestedSystems();

            ServiceLocator.Find<EventManager>().Propagate(new LevelStartedEvent(Base.gameManager.CurrentState as GameplayState), this);
        }


        private void ResetSystems()
        {
            DeactivateRequestedSystems();
            ActivateRequestedSystems();

            foreach (var system in gameplaySystems)
                system.Reset();


            DeactivateRequestedSystems();
            ActivateRequestedSystems();
        }

        public void Update(float dt)
        {
            ActivateRequestedSystems();

            foreach (var system in activeSystems)
                Update(system, dt);

            DeactivateRequestedSystems();

            frameCenteredSystemBlackBoard.Clear();
        }

        private void Update(GameplaySystem system, float dt)
        {
            //system.Update(dt);
            try
            {
                //UnityEngine.Profiling.Profiler.BeginSample(system.GetType().Name);
                system.Update(dt);
                //UnityEngine.Profiling.Profiler.EndSample();
            }
            catch (Exception e)
            {
                DebugPro.LogException<CoreGameplayLogTag>(message: $"Exception in system {system.GetType()}:\n{e}");
            }
        }

        private void DeactivateRequestedSystems()
        {
            foreach (var system in gameplaySystemsToDeactivate)
                activeSystems.Remove(system);

            gameplaySystemsToDeactivate.Clear();
        }

        private void ActivateRequestedSystems()
        {
            if (gameplaySystemsToActivate.Count == 0)
                return;

            foreach (var system in gameplaySystemsToActivate)
            {
                if (activeSystems.Contains(system) == false)
                {
                    activeSystems.Add(system);
                    system.OnActivated();
                }
            }

            activeSystems.Sort((a, b) => systemOrderings[a].CompareTo(systemOrderings[b]));

            gameplaySystemsToActivate.Clear();
        }

        public T GetSystem<T>() where T : IGameplaySystem
        {
            return (T) (IGameplaySystem) gameplaySystems.FirstOrDefault(s => s is T);
        }

        public void DeactivateSystem<T>() where T : IGameplaySystem
        {
            gameplaySystemsToDeactivate.Add(GetSystem<T>() as GameplaySystem);
        }

        public bool IsDeactive<T>() where T : IGameplaySystem
        {
            return activeSystems.Contains(GetSystem<T>() as GameplaySystem) == false;
        }


        public void ActivateSystem<T>() where T : IGameplaySystem
        {
            gameplaySystemsToActivate.Add(GetSystem<T>() as GameplaySystem);
        }
        
        public void ActivateSystem(GameplaySystem system)
        {
            gameplaySystemsToActivate.Add(system);
        }

        public GameBoard GameBoard()
        {
            return gameBoard;
        }


        public void AddPresentationHandler(PresentationHandler handler)
        {
            presentationHandlers.Add(handler);
        }

        public T GetPresentationHandler<T>() where T : PresentationHandler
        {
            foreach (var hanlder in presentationHandlers)
                if (hanlder is T)
                    return (T)hanlder;

            return default(T);
        }

        public void RemoveSystem<T>() where T : GameplaySystem
        {
            DeactivateSystem<T>();
            gameplaySystems.RemoveAll(g => g is T);
            foreach (var entry in systemTags)
                entry.Value.RemoveWhere(s => s is T);

        }

        public void DeactiveAllSystems()
        {
            gameplaySystemsToDeactivate.AddRange(activeSystems);
        }


        public InterSystemBlackBoard FrameCenteredBlackBoard()
        {
            return frameCenteredSystemBlackBoard;
        }

        public InterSystemBlackBoard SessionCenteredBlackBoard()
        {
            return sessionCenteredSystemBlackBoard;
        }


        public IEnumerable<IGameplaySystem> ActiveSystems()
        {
            return activeSystems;
        }
    }
}
