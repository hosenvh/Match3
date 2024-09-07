using System;
using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.Tiles;
using UnityEngine;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic;
using Match3.Utility.GolmoradLogging;
using static Match3.Game.Gameplay.QueryUtilities;
using static Match3.Game.Gameplay.ActionUtilites;


namespace Match3.Game.Gameplay.SubSystems.IceMakerMechanic
{
    public struct IceMakerPopingKeyType : KeyType
    {
    }

    public struct IceMakerRemovingKeyType : KeyType
    {
    }

    public interface IceMakerPresentationHandler : PresentationHandler
    {
        void StartPoping(IceMakerMainTile mainTile, List<CellStack> targets, Action<int> onTargetReached, Action onCompleted);
        void StartRemoving(IceMakerMainTile mainTile, Action onRemoveCompleted);
    }

    [Before(typeof(MatchDetectionSystem))]
    [Before(typeof(DestructionManagement.DestructionSystem))]
    [After(typeof(HitManagement.HitApplyingSystem))]
    public class IceMakerSystem : GameplaySystem
    {
        private const int PopCountPerIceMakerHit = 3;

        private readonly List<IceMakerMainTile> notRemovingIceMakers = new List<IceMakerMainTile>();
        private readonly List<IceMakerMainTile> readyToRemoveIceMakers = new List<IceMakerMainTile>();
        private readonly HashSet<IceMakerMainTile> readyToPopIceMakers = new HashSet<IceMakerMainTile>();
        private readonly HashSet<IceMakerMainTile> popingIceMakers = new HashSet<IceMakerMainTile>();

        private readonly RandomCellStackChooser randomCellStackChooser;
        private readonly IceMakerPresentationHandler presentationHandler;
        private readonly TileFactory tileFactory;

        public IceMakerSystem(GameplayController gameplayController) : base(gameplayController)
        {
            randomCellStackChooser = new RandomCellStackChooser(gameplayController.GameBoard().CellStackBoard());
            presentationHandler = gameplayController.GetPresentationHandler<IceMakerPresentationHandler>();
            tileFactory = ServiceLocator.Find<TileFactory>();

            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTileOnTop<IceMakerMainTile>(cellStack))
                    SetupIceMakerFor(TopTile(cellStack) as IceMakerMainTile);

            if (notRemovingIceMakers.IsEmpty())
                gameplayController.DeactivateSystem<IceMakerSystem>();
        }

        private void SetupIceMakerFor(IceMakerMainTile iceMaker)
        {
            iceMaker.onHit = () => { readyToPopIceMakers.Add(iceMaker); };
            notRemovingIceMakers.Add(iceMaker);
        }

        public override void Update(float dt)
        {
            ProcessReadyToPopIceMakers();
            ProcessNotRemovingIceMakers();
            ProcessReadyToRemoveIceMakers();
        }

        private void ProcessReadyToPopIceMakers()
        {
            if (readyToPopIceMakers.Count > 0)
            {
                GetSessionData<InputControlData>().AddLockedBy<IceMakerPopingKeyType>();
                if (GetFrameData<StabilityData>().wasStableLastChecked)
                    StartPoping();
            }
            else
                GetSessionData<InputControlData>().RemoveLockedBy<IceMakerPopingKeyType>();
        }

        private void StartPoping()
        {
            popingIceMakers.AddRange(readyToPopIceMakers);
            readyToPopIceMakers.Clear();

            var popingIceMakersIndexesCopy = new HashSet<IceMakerMainTile>(popingIceMakers);
            foreach (var iceMaker in popingIceMakersIndexesCopy)
            {
                int popCount = Mathf.Min(PopCountPerIceMakerHit, iceMaker.GetRemainedIceCount());
                var chosenCells = randomCellStackChooser.Choose(popCount, IsValidToChoose);
                foreach (CellStack cellStack in chosenCells)
                    FullyLock<IceMakerPopingKeyType>(cellStack);
                popCount = chosenCells.Count;

                iceMaker.PopIce(popCount);
                var targetList = chosenCells.GetRange(0, popCount);
                chosenCells.RemoveRange(0, popCount);

                var iceMakerCapture = iceMaker;
                if (targetList.Count > 0)
                {
                    presentationHandler.StartPoping(
                       iceMaker,
                       targetList,
                       i => CreateTileIn(targetList[i]),
                       () => FinishPopingIceMaker(iceMakerCapture));
                }
                else
                    FinishPopingIceMaker(iceMakerCapture);
            }
        }

        private bool IsValidToChoose(CellStack cellStack)
        {
            return
                cellStack.HasTileStack()
             && cellStack.CurrentTileStack().IsDepleted() == false
             && cellStack.CurrentTileStack().Top() is ColoredBead
             && IsFullyFree(cellStack)
             && IsSelectedByRainbow() == false
             && IsSelectedByPowerUps() == false;

            bool IsSelectedByRainbow()
            {
                if (cellStack.HasTileStack() && GetFrameData<RainbowActivationData>().tileStackHits.Contains(cellStack.CurrentTileStack()))
                    return true;

                foreach (var delayedCellStackHit in GetFrameData<RainbowActivationData>().delayedCellStackHits)
                    if (delayedCellStackHit.cellStack == cellStack)
                        return true;

                return false;
            }

            bool IsSelectedByPowerUps()
            {
                return GetFrameData<PowerUpActivationData>().affectedCellStacks.Contains(cellStack);
            }
        }

        private void CreateTileIn(CellStack cellStack)
        {
            if (cellStack == null || cellStack.HasTileStack() == false || cellStack.CurrentTileStack().IsDepleted())
                DebugPro.LogError<CoreGameplayLogTag>("[IceMaker] Target for ice is empty");

            gameplayController.creationUtility.PlaceTileInBoard(tileFactory.CreateIceTile(1), cellStack);
            FullyUnlock(cellStack);
        }

        private void FinishPopingIceMaker(IceMakerMainTile popingIceMaker)
        {
            popingIceMakers.Remove(popingIceMaker);
            if (popingIceMakers.Count == 0)
                GetSessionData<InputControlData>().RemoveLockedBy<IceMakerPopingKeyType>();
        }

        private void ProcessNotRemovingIceMakers()
        {
            for (int i = notRemovingIceMakers.Count - 1; i >= 0; --i)
            {
                var iceMaker = notRemovingIceMakers[i];
                if (iceMaker.IsEmpty())
                    AddReadyToRemoveIceMaker(iceMaker);
            }
        }

        private void AddReadyToRemoveIceMaker(IceMakerMainTile iceMaker)
        {
            readyToRemoveIceMakers.Add(iceMaker);
            notRemovingIceMakers.Remove(iceMaker);
        }

        private void ProcessReadyToRemoveIceMakers()
        {
            for (int i = readyToRemoveIceMakers.Count - 1; i >= 0; --i)
            {
                var iceMaker = readyToRemoveIceMakers[i];
                if (CanIceMakerBeRemoved(iceMaker))
                    StartRemoveIceMaker(iceMaker);
            }
        }

        private bool CanIceMakerBeRemoved(IceMakerMainTile iceMaker)
        {
            return !IsInProcess(iceMaker) &&
                   iceMaker.IsEmpty() &&
                   IsFullyFree(iceMaker.Parent().Parent());
        }

        private bool IsInProcess(IceMakerMainTile iceMaker)
        {
            return readyToPopIceMakers.Contains(iceMaker) || popingIceMakers.Contains(iceMaker);
        }

        private void StartRemoveIceMaker(IceMakerMainTile iceMaker)
        {
            readyToRemoveIceMakers.Remove(iceMaker);
            LockIceMaker<IceMakerRemovingKeyType>(iceMaker);
            presentationHandler.StartRemoving(iceMaker, onRemoveCompleted: () => Remove(iceMaker));

            if (notRemovingIceMakers.IsEmpty() && readyToRemoveIceMakers.IsEmpty())
                gameplayController.DeactivateSystem<IceMakerSystem>();
        }

        private void Remove(IceMakerMainTile iceMaker)
        {
            UnlockIceMaker(iceMaker);
            iceMaker.SetAsDestroyed();
            var destructionData = GetFrameData<DestructionData>();

            FullyDestroy(iceMaker);
            destructionData.tilesToDestroy.Add(iceMaker);

            foreach (var slave in iceMaker.Slaves())
            {
                FullyDestroy(slave);
                destructionData.tilesToDestroy.Add(slave);
            }
        }

        private void LockIceMaker<T>(IceMakerMainTile iceMaker) where T : KeyType
        {
            FullyLock<T>(iceMaker.Parent().Parent());
            foreach (var slave in iceMaker.Slaves())
                FullyLock<T>(slave.Parent().Parent());
        }

        private void UnlockIceMaker(IceMakerMainTile iceMaker)
        {
            FullyUnlock(iceMaker.Parent().Parent());
            foreach (var slave in iceMaker.Slaves())
                FullyUnlock(slave.Parent().Parent());
        }
    }
}