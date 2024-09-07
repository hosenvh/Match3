
using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ArtifactMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ExplosionManagement;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic;
using Match3.Game.Gameplay.Swapping;
using Match3.Game.Gameplay.Tiles;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.RainbowMechanic
{
    public interface RainbowActivationPresentationHandler : PresentationHandler
    {
        void HandleSingleRainbowActivation(Rainbow rainbow, List<TileStack> targets, Action onCompleted);
        void HandleDoubleRainbowActivation(Rainbow rainbow1, Rainbow rainbow2, Action onCompleted);
    }

    public struct RainbowActivateKeyType : KeyType
    { }

    public struct SingleRainbowActivatedEvent : GameEvent
    {
        public readonly List<TileStack> targets;

        public SingleRainbowActivatedEvent(List<TileStack> targets)
        {
            this.targets = targets;
        }
    }

    public struct DoubleRainbowActivatedEvent : GameEvent
    {

    }

    public class RainbowActivationGatheringSystem : GameplaySystem
    {
        List<TileStack> gatheredTileStacks = new List<TileStack>();

        public RainbowActivationGatheringSystem(GameplayController gameplayController) : base(gameplayController)
        {
        }

        public override void Update(float dt)
        {
            if (gatheredTileStacks.Count > 0)
            {
                GetFrameData<DirectRainbowActivationRequestData>().targets.AddRange(gatheredTileStacks);
                gatheredTileStacks.Clear();
            }
        }

        public void Enqueue(TileStack tileStack)
        {
            gatheredTileStacks.Add(tileStack);
        }
    }

    // TODO: Move out the target finding to another class.
    [After(typeof(Swapping.SwapExecutionSystem))]
    [Before(typeof(ExplosionManagement.ExplosionActivationSystem))]
    public class RainbowActivationSystem : GameplaySystem
    {
        const float hitPropagationDelay = 0.06f;


        GridIterator<CellStack> cellStackBoardIterator;

        public RainbowActivationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            cellStackBoardIterator = gameplayController.GameBoard().DefaultCellBoardIterator();
        }

        public override void Update(float dt)
        {
            foreach (var swapData in GetFrameData<ExecutedSwapsData>().data)
            {
                if (swapData.originTarget.HasTileStack() == false || swapData.destinationTarget.HasTileStack() == false)
                    return;

                bool isOriginRainbow = HasRainbow(swapData.originTarget);
                bool isDestinationRainbow = HasRainbow(swapData.destinationTarget);

                if (isOriginRainbow && isDestinationRainbow)
                    TryDoubleActivate(swapData.originTarget.CurrentTileStack(), swapData.destinationTarget.CurrentTileStack());
                else if(isOriginRainbow)
                    TrySingleAtivate(swapData.originTarget.CurrentTileStack(), swapData.destinationTarget.CurrentTileStack());
                else if (isDestinationRainbow)
                    TrySingleAtivate(swapData.destinationTarget.CurrentTileStack(), swapData.originTarget.CurrentTileStack());

            }

            foreach (var directTarget in GetFrameData<DirectRainbowActivationRequestData>().targets)
            {

                var target =
                    new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard()).
                    ChooseOne(IsValidForDirectionActivation);

                if(target != null)
                    TrySingleAtivate(directTarget, target.CurrentTileStack());
                else
                    ApplySingleRainbowActivation(directTarget.Top() as Rainbow, new List<TileStack>());

            }
        }

        bool IsValidForDirectionActivation(CellStack cellStack)
        {
            return cellStack.HasTileStack()
                        && IsMatchable(cellStack.CurrentTileStack())
                        && IsRainbow(cellStack.CurrentTileStack()) == false
                        && QueryUtilities.IsFullyFree(cellStack);
        }

        void TryDoubleActivate(TileStack origin, TileStack destination)
        {
            var rainbowTile1 = origin.Top() as Rainbow;
            var rainbowTile2 = destination.Top() as Rainbow;

            if (rainbowTile1.IsActivated() || rainbowTile2.IsActivated())
                return;
            rainbowTile1.MarkAsActivated();
            rainbowTile2.MarkAsActivated();

            GetFrameData<RainbowActivationData>().activeRainbowes.Add(rainbowTile1);
            GetFrameData<RainbowActivationData>().activeRainbowes.Add(rainbowTile2);

            var targets = new List<CellStack>();
            foreach (var element in cellStackBoardIterator)
            {
                if (IsValidForDoubleRainbow(element.value) 
                    && element.value.CurrentTileStack() != origin 
                    && element.value.CurrentTileStack() != destination)

                    targets.Add(element.value);
            }

            origin.GetComponent<LockState>().LockBy<RainbowActivateKeyType>();
            destination.GetComponent<LockState>().LockBy<RainbowActivateKeyType>();

            foreach (var cellStack in targets)
                if (cellStack.HasTileStack())
                    cellStack.CurrentTileStack().GetComponent<LockState>().LockBy<RainbowActivateKeyType>();

            gameplayController.GetPresentationHandler<RainbowActivationPresentationHandler>().
                    HandleDoubleRainbowActivation(
                rainbowTile1,
                rainbowTile2,
                () => ApplyDoubleRainbowActivation(rainbowTile1, rainbowTile2, targets));

            ServiceLocator.Find<EventManager>().Propagate(new DoubleRainbowActivatedEvent(), this);
        }

        private void TrySingleAtivate(TileStack rainbowStack, TileStack targetStack)
        {
            var rainbowTile = rainbowStack.Top() as Rainbow;
            if(rainbowTile.IsActivated() == false && IsMatchable(targetStack))
            {
                rainbowTile.MarkAsActivated();

                GetFrameData<RainbowActivationData>().activeRainbowes.Add(rainbowTile);
                var targets = FindSingleRainbowTargetsFor(targetStack);

                rainbowStack.GetComponent<LockState>().LockBy<RainbowActivateKeyType>();

                foreach (var tileStack in targets)
                    tileStack.GetComponent<LockState>().LockBy<RainbowActivateKeyType>();

                gameplayController.GetPresentationHandler<RainbowActivationPresentationHandler>().
                    HandleSingleRainbowActivation(rainbowTile, targets, () => ApplySingleRainbowActivation(rainbowTile, targets));

                ServiceLocator.Find<EventManager>().Propagate(new SingleRainbowActivatedEvent(targets), this);
            }
        }

        private void ApplySingleRainbowActivation(Rainbow rainbowTile, List<TileStack> targets)
        {
            RainbowActivationData hitData = GetFrameData<RainbowActivationData>();


            hitData.activatedRainbowCellStacks.Add(rainbowTile.Parent().Parent());

            DestroyRainbow(rainbowTile);

            foreach (var tileStack in targets)
                tileStack.GetComponent<LockState>().Release();

            hitData.tileStackHits.AddRange(targets);
        }

        private void ApplyDoubleRainbowActivation(Rainbow rainbowTile1, Rainbow rainbowTile2, List<CellStack> targets)
        {
   
            var hitData = GetFrameData<RainbowActivationData>();

            hitData.activatedRainbowCellStacks.Add(rainbowTile1.Parent().Parent());
            hitData.activatedRainbowCellStacks.Add(rainbowTile2.Parent().Parent());


            DestroyRainbow(rainbowTile1);
            DestroyRainbow(rainbowTile2);

            foreach (var cellStack in targets)
                if (cellStack.HasTileStack())
                {
                    cellStack.CurrentTileStack().GetComponent<LockState>().Release();
                    TryActivateExplosiveIn(cause: rainbowTile1, cellStack.CurrentTileStack(), DistanceOf(rainbowTile1.Parent().Parent(), cellStack));
                }


            var data = GetFrameData<RainbowActivationData>();
            foreach (var cellStack in targets)
                data.delayedCellStackHits.Add(
                    new DelayedCellHitData(
                        cellStack,
                        hitPropagationDelay * DistanceOf(rainbowTile1.Parent().Parent(), cellStack)));

        }

        private void TryActivateExplosiveIn(Tile cause, TileStack tileStack, int distance)
        {
            if (tileStack.IsDepleted() == false && tileStack.Top() is ExplosiveTile)
                GetFrameData<InternalExplosionActivationData>().delayedTargets.
                    Add(new DelayedActivationData(tileStack, cause, 0)); // Note: We are setting delay to zero because non zero values may cause missed combo counting, A better way should be find
        }

        void DestroyRainbow(Rainbow rainbowTile)
        {
            GetFrameData<DestructionData>().tilesToDestroy.Add(rainbowTile);
            rainbowTile.Parent().Pop();
            rainbowTile.Parent().GetComponent<LockState>().Release();
            GetFrameData<RainbowActivationData>().activeRainbowes.Remove(rainbowTile);
        }

        private List<TileStack> FindSingleRainbowTargetsFor(TileStack tileStack)
        {
            var targets = new List<TileStack>();

            foreach(var element in cellStackBoardIterator)
            {
                if (IsValidForSingleRainbow(element.value) && Matches(element.value, tileStack))
                    targets.Add(element.value.CurrentTileStack());
            }

            return targets;

        }

        private bool IsValidForSingleRainbow(CellStack value)
        {
            return value.HasTileStack() 
                && value.CurrentTileStack().IsDepleted() == false 
                && value.CurrentTileStack().GetComponent<LockState>().IsFree() 
                && MatchMadeFor(value.CurrentTileStack()) == false;
        }

        private bool IsValidForDoubleRainbow(CellStack value)
        {
            return value.Top() is EmptyCell == false 
                &&(value.HasTileStack() == false || value.CurrentTileStack().GetComponent<LockState>().IsFree());
        }


        bool MatchMadeFor(TileStack target)
        {
            foreach (var match in GetFrameData<CreatedMatchesData>().data)
                if (match.tileStacks.Contains(target))
                    return true;
            return false;
        }

        // TODO: This is similar to the functionality of MatchingRulesTable. Try refactor this.
        private bool Matches(CellStack cellStack, TileStack targetTileStack)
        {
            if (cellStack.HasTileStack() == false)
                return false;

            var tileStack = cellStack.CurrentTileStack();

            foreach (var tile in tileStack.Stack())
            {
                if (Maches(tile, targetTileStack.Top()))
                    return true;

                else if (tile.GetComponent<TileMatchingProperties>().allowsMatchFallThrough == false)
                    return false;
            }

            return false;

        }

        private bool Maches(Tile tile, Tile targetTile)
        {
            var currentRainbowComp = tile.GetComponent<TileRainbowProperties>();
            switch (currentRainbowComp.matchingRule)
            {
                case TileRainbowProperties.MatchingRule.None:
                    return false;

                case TileRainbowProperties.MatchingRule.MatchWithRainbowFindOtherMatchesBySameColor:
                    var currentColorComp = tile.GetComponent<TileColorComponent>();
                    var targetColorComp = targetTile.GetComponent<TileColorComponent>();
                    return currentColorComp != null && targetColorComp != null && currentColorComp.color == targetColorComp.color;

                case TileRainbowProperties.MatchingRule.MatchWithRainbowFindOtherMatchesBySameType:
                    return targetTile.GetType() == tile.GetType();

                default:
                    return false;
            }
        }

        private bool IsMatchable(TileStack stack)
        {
            return stack.IsDepleted() == false && stack.Top().GetComponent<TileRainbowProperties>().DoesMatchWithRainbow();
        }

        private bool HasRainbow(CellStack cellStack)
        {
            return cellStack.HasTileStack() && IsRainbow(cellStack.CurrentTileStack());
        }
        private bool IsRainbow(TileStack stack)
        {
            return stack.IsDepleted() == false && stack.Top() is Rainbow;
        }
    }
}