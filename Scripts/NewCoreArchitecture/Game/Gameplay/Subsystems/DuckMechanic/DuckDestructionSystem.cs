using System;
using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.TileGeneration;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using static Match3.Game.Gameplay.QueryUtilities;
using static Match3.Game.Gameplay.ActionUtilites;
using Match3.Presentation.Gameplay.Tiles;

namespace Match3.Game.Gameplay.SubSystems.DuckMechanic
{
    public struct DuckDestructionKeyType : KeyType
    { }

    public interface DuckDestructionPresentationHandler : PresentationHandler
    {
        void PlaceGeneratedTilesIn (Duck duck, CellStack origin, List<CellStack> targets, Action<int> onTargetReached, Action onCompleted);
        void StartRemoving(Duck duck);
    }

    [Before(typeof(DuckMovementSystem))]
    [Before(typeof(DuckGenerationSystem))]
    public class DuckDestructionSystem : DuckSystem
    {
        private readonly HashSet<Duck> destroyDucks = new HashSet<Duck>();
        private readonly HashSet<Duck> pendingDestroyDucks = new HashSet<Duck>();
        private DuckDestructionPresentationHandler presentationHandler;
        private RandomCellStackChooser randomCellStackChooser;

        public DuckDestructionSystem (GameplayController gameplayController) : base (gameplayController)
        {
            randomCellStackChooser = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard());
            presentationHandler = gameplayController.GetPresentationHandler<DuckDestructionPresentationHandler>();
            FetchDuckTilesFromBoard();
        }

        private void FetchDuckTilesFromBoard ()
        {
            ducks = new List<Duck>();
            scheduledGenerationData = new List<GenerationData>();

            foreach (CellStack cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTile<Duck>(cellStack))
                    ducks.Add(FindTile<Duck>(cellStack));
        }

        public override void Update (float dt)
        {
            UpdateDucksForDestroy();  
            TryToStartGenerationItemForReadyToDestroyDucks();
            UpdateInputControlDataLockState();
        }

        private void UpdateDucksForDestroy ()
        {
            int totalDestroyed = 0;

            foreach (Duck duck in ducks)
            {
                if (duck.ShouldGetReadyForGeneration())
                {
                    pendingDestroyDucks.Add(duck);
                    totalDestroyed++;
                }
            }

            totalDucksCountInBoard -= totalDestroyed;
            if (totalDestroyed > 0)
                TryScheduleGeneration(GENERATION_DEFAULT_TURNS_TO_WAIT_COUNT, totalDestroyed);

            foreach (Duck pendingDestroyDuck in pendingDestroyDucks)
                ducks.Remove(pendingDestroyDuck);
        }

        private void TryToStartGenerationItemForReadyToDestroyDucks ()
        {
            if (CanStartGenerationItemForReadyToDestroyDucks())
                StartGenerationItemForReadyToDestroyDucks();
        }

        private bool CanStartGenerationItemForReadyToDestroyDucks ()
        {
            return pendingDestroyDucks.Count > 0 && GetFrameData<StabilityData>().wasStableLastChecked;
        }

        private void StartGenerationItemForReadyToDestroyDucks ()
        {
            MoveAllPendingDestroyDucksToDestroyDucksList();

            List<CellStack> targetCells = ChooseDestroyDucksTargets(Duck.GENERATION_AMOUNT_PER_DUCK * destroyDucks.Count);
            LockCellStacksAsDuckDestroyTargets(targetCells);

            HashSet<Duck> destroyDucksCopy = new HashSet<Duck>(destroyDucks);
            foreach (Duck duck in destroyDucksCopy)
            {
                List<CellStack> targetCellsForThisDuck = CutListElements(targetCells, Duck.GENERATION_AMOUNT_PER_DUCK);
                StartGenerationItemForDuck(duck, targetCellsForThisDuck);
            }
        }

        private void MoveAllPendingDestroyDucksToDestroyDucksList ()
        {
            destroyDucks.AddRange(pendingDestroyDucks);
            pendingDestroyDucks.Clear();
        }

        private List<CellStack> ChooseDestroyDucksTargets (int neededTargetsCount)
        {
            return randomCellStackChooser.Choose(neededTargetsCount, IsValidToChooseAsDuckDestroyTargetCellStack);
        }

        private bool IsValidToChooseAsDuckDestroyTargetCellStack (CellStack cellStack)
        {
            return
                cellStack.HasTileStack()
                && cellStack.CurrentTileStack().IsDepleted() == false
                && cellStack.CurrentTileStack().Top() is ColoredBead;
        }

        private void LockCellStacksAsDuckDestroyTargets (List<CellStack> cellStacks)
        {
            foreach (CellStack cellStack in cellStacks)
                Lock<DuckDestructionKeyType>(cellStack.CurrentTileStack());
        }

        private void StartGenerationItemForDuck (Duck duck, List<CellStack> targetCellsForThisDuck)
        {
            FullyLock<DuckDestructionKeyType>(duck.Parent().Parent());

            if (targetCellsForThisDuck.Count > 0)
                presentationHandler.PlaceGeneratedTilesIn(
                    duck: duck,
                    origin: duck.Parent().Parent(),
                    targets: targetCellsForThisDuck,
                    onTargetReached: i => CreateTileIn(targetCellsForThisDuck[i], duck.GetChildItemAtIndex(i)),
                    onCompleted: () => RemoveDestroyedDuck(duck));
            else // TODO: Review this line. Maybe need to remove
                RemoveDestroyedDuck(duck);

            presentationHandler.StartRemoving(duck);
        }

        private void CreateTileIn (CellStack cellStack, DuckItem duckItem)
        {
            gameplayController.creationUtility.ReplaceTileInBoard(CreateTileItem(duckItem.Item, duckItem.Color), cellStack);
        }

        private Tile CreateTileItem (Type itemType, TileColor color)
        {
            if (itemType == typeof(Tiles.RocketBox))
                return tileFactory.CreateRocketBoxTile(color);
            else if (itemType == typeof(Tiles.Explosives.Rocket))
                return tileFactory.CreateRocketTile();
            else if (itemType == typeof(Tiles.Nut))
                return tileFactory.CreateNutTile(1);
            else if (itemType == typeof(Tiles.Explosives.Bomb))
                return tileFactory.CreateBombTile();
            else if (itemType == typeof(Tiles.Explosives.Dynamite))
                return tileFactory.CreateDynamiteTile();
            else if (itemType == typeof(Tiles.Explosives.TNTBarrel))
                return tileFactory.CreateTNTBarrelTile();
            else if (itemType == typeof(Tiles.Rainbow))
                return tileFactory.CreateRainbowTile();
            else if (itemType == typeof(Tiles.HoneyJar))
                return tileFactory.CreateHoneyJarTile();
            else if (itemType == typeof(Tiles.JamJar))
                return tileFactory.CreateJamJarTile();

            throw new Exception($"Tile {itemType.Name} is not supported by {GetType()} yet");
        }

        private void RemoveDestroyedDuck (Duck duck)
        {
            FullyUnlock(duck.Parent().Parent());
            FullyDestroy(duck);
            GetFrameData<DestructionData>().tilesToDestroy.Add(duck);

            duck.MarkAsDestroyed();
            destroyDucks.Remove(duck);
        }

        private List<T> CutListElements<T> (List<T> from, int neededCount)
        {
            List<T> result = from.GetRange(0, Math.Min(neededCount, from.Count));
            from.RemoveRange(0, result.Count);
            return result;
        }

        private void UpdateInputControlDataLockState ()
        {
            if (ShouldLockInputControlData())
                GetSessionData<InputControlData>().AddLockedBy<DuckDestructionKeyType>();
            else
                GetSessionData<InputControlData>().RemoveLockedBy<DuckDestructionKeyType>();
        }

        private bool ShouldLockInputControlData()
        {
            return pendingDestroyDucks.Count > 0 || destroyDucks.Count > 0;
        }
    }
}