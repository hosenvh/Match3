

using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.SubSystemsData.FrameData.HoneyExpansion;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Linq;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.HoneyMechanic
{
    public interface HoneyExpansionPresentationHandler : PresentationHandler
    {
        void HandleGrowth(CellStack cellstack, Action onCompleted);
    }

    public struct HoneySystemKeyType : KeyType
    {

    }

    [After(typeof(DestructionManagement.DestructionSystem))]
    public class HoneyExpansionSystem : GameplaySystem
    {
        DestroyedObjectsData destroyedObjectsData;

        List<CellStack> honeyCellStacks = new List<CellStack>();
        List<CellStack> honeyJarCellStacks = new List<CellStack>();
        List<CellStack> honeycombCellStacks = new List<CellStack>();
        List<CellStack> hardenedHoneyCellStacks = new List<CellStack>();

        HashSet<CellStack> validCellStacksToGrow = new HashSet<CellStack>();
        CellStackBoard cellStackBoard;

        TileFactory tileFactory;
        RandomCellStackChooser randomCellStackChooser;
        CellStack[] cellStacks;

        HoneyExpansionPresentationHandler presentationHandler;
        HoneyGenerationData honeygenerationData;
        HoneyExpansionActivatingData honeyExpansionActivatingData;

        HoneyExpansionSystemState currentState;

        OnlyHoneycombsLeftExpansionState onlyHoneycombsLeftState;
        HoneyInBoardExpansionState honeyInBoardState;
        EmptyExpansionState disabledState;

        public HoneyExpansionSystem(GameplayController gameplayController) : base(gameplayController)
        {
            tileFactory = ServiceLocator.Find<TileFactory>();

            cellStackBoard = gameplayController.GameBoard().CellStackBoard();

            destroyedObjectsData = GetFrameData<DestroyedObjectsData>();
            honeygenerationData = GetSessionData<HoneyGenerationData>();
            honeyExpansionActivatingData = GetFrameData<HoneyExpansionActivatingData>();

            randomCellStackChooser = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard());
            cellStacks = gameplayController.GameBoard().ArrbitrayCellStackArray();

            presentationHandler = gameplayController.GetPresentationHandler<HoneyExpansionPresentationHandler>();

            honeyInBoardState = new HoneyInBoardExpansionState(this, cellStackBoard, ref honeyCellStacks, ref hardenedHoneyCellStacks);
            onlyHoneycombsLeftState = new OnlyHoneycombsLeftExpansionState(this, cellStackBoard);
            disabledState = new EmptyExpansionState(this, cellStackBoard, gameplayController);

        }

        public override void Start()
        {
            UpdateHoneycombCellStacks();
            UpdateHoneyCellStacks();
            UpdateCurrentState();
        }

        private void UpdateHoneycombCellStacks()
        {
            foreach (var cellStack in cellStacks)
            {
                if (HasTile<Honeycomb>(cellStack))
                    honeycombCellStacks.Add(cellStack);
            }
        }

        // NOTE: It is assumed that UpdateHoneyCellStacks is effectively executed before this method.
        // TODO: Try to remove this dependency
        public bool ShouldBeDisabled()
        {
            return honeycombCellStacks.Count == 0
                && honeyCellStacks.Count == 0
                && honeyJarCellStacks.Count == 0
                && honeygenerationData.honeyTilesBeingGenerated == 0
                && honeyExpansionActivatingData.areThereAnyActivatingItems == false;
        }

        public override void Update(float dt)
        {
            if(destroyedObjectsData.tiles.Count > 0)
                UpdateHoneyCellStacks();

            UpdateCurrentState();
            currentState.Update();
        }

        private void UpdateCurrentState()
        {
            if (ShouldBeDisabled())
                SetState(disabledState);
            else if (honeyCellStacks.Count == 0 && honeycombCellStacks.Count > 0)
                    SetState(onlyHoneycombsLeftState);
            else
                SetState(honeyInBoardState);
        }

        void SetState(HoneyExpansionSystemState state)
        {
            if (currentState != state)
            {
                currentState = state;
                currentState.OnEnter();
            }
        }

        public void UpdateHoneyCellStacks()
        {
            honeyCellStacks.Clear();
            honeyJarCellStacks.Clear();
            hardenedHoneyCellStacks.Clear();

            foreach (var cellStack in cellStacks)
            {
                if (HasTile<Honey>(cellStack))
                    honeyCellStacks.Add(cellStack);
                if (HasTile<HoneyJar>(cellStack))
                    honeyJarCellStacks.Add(cellStack);
                if (HasTile<HardenedHoney>(cellStack))
                    hardenedHoneyCellStacks.Add(cellStack);
            }          
        }

        public void StartGrowingAHoneyTile()
        {
            UpdateValidCellStackForGrowing();

            var cellStack =  randomCellStackChooser.ChooseOne(IsCellStackValidForGrowing);

            if (cellStack != null)
            {
                if(cellStack.HasTileStack())
                    ActionUtilites.Lock<HoneySystemKeyType>(cellStack.CurrentTileStack());
                presentationHandler.HandleGrowth(cellStack, () => ApplyHoneyGrowth(cellStack));
            }
            else
                GetSessionData<InputControlData>().RemoveLockedBy<HoneySystemKeyType>();
        }

        void ApplyHoneyGrowth(CellStack cellStack)
        {
            if (cellStack.HasTileStack())
                ActionUtilites.Unlock(cellStack.CurrentTileStack());
            gameplayController.creationUtility.PlaceTileInBoard(tileFactory.CreateHoneyTile(), cellStack);
            honeyCellStacks.Add(cellStack);
            GetSessionData<InputControlData>().RemoveLockedBy<HoneySystemKeyType>();
        }

        private void UpdateValidCellStackForGrowing()
        {
            validCellStacksToGrow.Clear();

  
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
            {
                if (HasTile<Honey>(cellStack) || HasTileOnTop<Honeycomb>(cellStack))
                {
                    TryAddAsValidForGrowing(cellStack, Direction.Down);
                    TryAddAsValidForGrowing(cellStack, Direction.Up);
                    TryAddAsValidForGrowing(cellStack, Direction.Left);
                    TryAddAsValidForGrowing(cellStack, Direction.Right);

                }
            }
        }

        // TODO: REFACTOR THIS FUCKING FUCK.
        private void TryAddAsValidForGrowing(CellStack honeyCellStack, Direction direction)
        {
            if (gameplayController.boardBlockageController.IsBlocked(honeyCellStack, direction))
                return;

            var cellStack = cellStackBoard.DirectionalElementOf(honeyCellStack.Position(), direction);

            if (cellStack == null || cellStack.Top().CanContainTile() == false)
                return;

            var tileStack = cellStack.CurrentTileStack();

            if (tileStack == null)
                validCellStacksToGrow.Add(cellStack);
            else if (tileStack.GetComponent<LockState>().IsFree() == false)
                return;
            else if(tileStack.IsDepleted())
                validCellStacksToGrow.Add(cellStack);
            else if(HasTile<Honey>(cellStack) == false && tileStack.Top().GetComponent<TileHoneyMechanicProperties>().canBeTakenOverByHoney)
                validCellStacksToGrow.Add(cellStack);
        }


        bool IsCellStackValidForGrowing(CellStack cellStack)
        {
            return validCellStacksToGrow.Contains(cellStack);
        }
    }
}