

using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Main;


namespace Match3.Game.Gameplay.RainbowMechanic
{

    public class RainbowGeerationKeyType : KeyType
    {

    }

    public struct RainbowGeneratedEvent : GameEvent
    {

    }

    public interface RainbowGenerationPresentationHandler : PresentationHandler
    {
        void HandleEnergyFlow(ExplosiveTile tile, Action onCompleted);
        void MoveTo(Type tileTypeToMove, CellStack target, Action onPlacedAction);
        void HandleCombo(Tile tile, int comboNumber);
    }



    // TODO: Refactor this shit.
    [After(typeof(DestructionManagement.DestructionSystem))]
    public class RainbowGenerationSystem : GameplaySystem
    {
        public class TileToCreate
        {
            public Type TileType { get; }
            public Func<Tile> TileCreation { get; }

            public TileToCreate(Type tileType, Func<Tile> tileCreation)
            {
                TileType = tileType;
                TileCreation = tileCreation;
            }
        }

        public event Action<int> OnTileExplosion = delegate {  };
        
        float currentFillValue = 0;

        float[] chainValueFactors = { 1, 1.5f, 1.65f };


        Dictionary<Type, float> explosiveValues = new Dictionary<Type, float>();


        ExplosionGroupDeterminer explosionGroupDeterminer = new ExplosionGroupDeterminer();
        Dictionary<ExplosiveTile, float> explosivesFactors = new Dictionary<ExplosiveTile, float>();

        HashSet<CellStack> forbiddenCellStacks = new HashSet<CellStack>();

        HashSet<CellStack> rainbowTargets = new HashSet<CellStack>();
        int toCreateRainbows = 0;
        int pendingTilesToCreate = 0;

        RainbowGenerationPresentationHandler presentationHandler;

        RandomCellStackChooser randomCellStackChooser;

        List<GoalTargetType> goalTargetTypes = new List<GoalTargetType>();
        List<TileToCreate> bonusTiles = new List<TileToCreate>();

        public RainbowGenerationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            foreach (var element in gameplayController.GameBoard().DefaultCellBoardIterator())
                if (element.value.Top() is EmptyCell || element.value.HasAttachment<RainbowPreventer>())
                    forbiddenCellStacks.Add(element.value);

            presentationHandler = gameplayController.GetPresentationHandler<RainbowGenerationPresentationHandler>();
            randomCellStackChooser = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard());
        }

        public void SetExplosiveValue<T>(float value) where T : ExplosiveTile
        {
            explosiveValues[typeof(T)] = value;
        }

        public void AddBonusTileTypeToCreateWithEachRainBow(TileToCreate bonusTile)
        {
            bonusTiles.Add(bonusTile);
        }

        public override void Start()
        {
            foreach (var goalTracker in gameplayController.GetSystem<LevelStoppingSystem>().AllGoals())
                goalTargetTypes.Add(goalTracker.goalType);

            base.Start();
        }

        public override void Update(float dt)
        {
            explosionGroupDeterminer.Determine(GetFrameData<ExplosionActivationData>().processedExplosives);

            foreach (var group in explosionGroupDeterminer.Groups())
            {
                var lenght = group.tiles.Count;
                foreach (var tile in group.tiles)
                    if (tile is ExplosiveTile explosiveTile)
                        explosivesFactors[explosiveTile] = FactorForChainOfLenght(lenght);

                presentationHandler.HandleCombo(group.rootCause, lenght);
                OnTileExplosion.Invoke(lenght);
            }


            foreach (var tile in GetFrameData<DestroyedObjectsData>().tiles)
                if (tile is ExplosiveTile explosiveTile)
                    FillFor(explosiveTile);


            if (currentFillValue >= 1)
                SetupRainbowCreation();

            if (toCreateRainbows > 0 && GetFrameData<StabilityData>().wasStableLastChecked)
            {
                GetFrameData<StabilityData>().wasStableLastChecked = false;
                CreateRainbows();
            }

        }

        private float FactorForChainOfLenght(int lenght)
        {
            return chainValueFactors[UnityEngine.Mathf.Min(lenght - 1, chainValueFactors.Length - 1)];
        }

        public void SetupRainbowCreation()
        {
            int rainbowsToCreate = UnityEngine.Mathf.FloorToInt(currentFillValue);

            currentFillValue -= rainbowsToCreate;

            toCreateRainbows += rainbowsToCreate;

            GetSessionData<InputControlData>().AddLockedBy<RainbowGeerationKeyType>();
        }

        void FillFor(ExplosiveTile explosiveTile)
        {
            presentationHandler.HandleEnergyFlow(explosiveTile, () => AddValueOf(explosiveTile));
        }

        void AddValueOf(ExplosiveTile explosiveTile)
        {
            currentFillValue += explosiveValues[explosiveTile.GetType()] * explosivesFactors[explosiveTile];

            explosivesFactors.Remove(explosiveTile);
        }

        void CreateRainbows()
        {
            List<TileToCreate> tilesToCreate = GetTilesToCreate();
            var cellStacks = randomCellStackChooser.Choose(tilesToCreate.Count, IsAvailableForRainbow);

            for (int index = 0; index < cellStacks.Count; index++)
            {
                var targetCellStack = cellStacks[index];
                var tileToCreate = tilesToCreate[index];

                pendingTilesToCreate++;
                rainbowTargets.Add(targetCellStack);
                presentationHandler.MoveTo(tileToCreate.TileType, targetCellStack, onPlacedAction: () => ReplaceWithTile(tileToCreate, targetCellStack));
                if (targetCellStack.HasTileStack())
                    targetCellStack.CurrentTileStack().GetComponent<LockState>().LockBy<RainbowGeerationKeyType>();

                ServiceLocator.Find<EventManager>().Propagate(new RainbowGeneratedEvent(), this);
            }

            toCreateRainbows = 0;
            TryUnlockInput();

            List<TileToCreate> GetTilesToCreate()
            {
                List<TileToCreate> result = new List<TileToCreate>();
                for (int i = 0; i < toCreateRainbows; i++)
                    result.Add(new TileToCreate(tileType: typeof(Rainbow), tileCreation: ServiceLocator.Find<MainTileFactory>().CreateRainbowTile));
                for (int i = 0; i < toCreateRainbows; i++)
                    result.AddRange(bonusTiles);
                return result;
            }
        }

        private bool IsAvailableForRainbow(CellStack cellStack)
        {
            return
                forbiddenCellStacks.Contains(cellStack) == false
                && rainbowTargets.Contains(cellStack) == false
                && cellStack.HasTileStack()
                && cellStack.CurrentTileStack().IsDepleted() == false
                && cellStack.CurrentTileStack().Top() is ColoredBead coloredBead
                && cellStack.CurrentTileStack().GetComponent<LockState>().IsFree()
                && IsNotGoal(coloredBead);
        }

        private bool IsNotGoal(ColoredBead coloredBead)
        {
            foreach (var goalType in goalTargetTypes)
                if (goalType.Includes(coloredBead))
                    return false;

            return true;
        }


        // TODO: Refactor this shit.
        private void ReplaceWithTile(TileToCreate tileToCreate, CellStack cellStack)
        {
            rainbowTargets.Remove(cellStack);
            if (cellStack.HasTileStack())
            {
                cellStack.CurrentTileStack().Destroy();
            }

            var factory = ServiceLocator.Find<TileFactory>();
            var tileStack = factory.CreateTileStack();
            tileStack.Push(tileToCreate.TileCreation.Invoke());

            cellStack.SetCurrnetTileStack(tileStack);
            tileStack.SetPosition(cellStack.Position());

            pendingTilesToCreate--;
            TryUnlockInput();
            ServiceLocator.Find<EventManager>().Propagate(new TileStackGeneratedEvent(tileStack), this);
        }

        void TryUnlockInput()
        {
            if (pendingTilesToCreate <= 0)
                GetSessionData<InputControlData>().RemoveLockedBy<RainbowGeerationKeyType>();
        }

        public float CurrentFillAmount()
        {
            return currentFillValue;
        }

        public bool HasToBeCreatedRainbow()
        {
            return toCreateRainbows > 0;
        }

        public List<Type> GetBonusTilesTypes()
        {
            return bonusTiles.Select(bonusTile => bonusTile.TileType).ToList();
        }
    }
}