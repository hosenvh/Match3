using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;
using static Match3.Game.Gameplay.ActionUtilites;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ExplosionManagement;

namespace Match3.Game.Gameplay.SubSystems.VacuumCleanerMechanic
{

    public interface VacuumCleanerActivationPresentationHandler : PresentationHandler
    {
        void Activate(VacuumCleaner vacuumCleaner, List<CellStack> list, Action<CellStack> onCellStackReached, Action onCompleted);
    }

    public struct VacuumCleanerActivationKeyType : KeyType
    {

    }

    [Before(typeof(HitManagement.HitGenerationSystem))]
    public class VacuumCleanerActivationSystem : GameplaySystem
    {
        List<VacuumCleaner> notActivatedVacuumCleaners = new List<VacuumCleaner>();
        List<VacuumCleaner> pendingVacuumCleaners = new List<VacuumCleaner>();

        Dictionary<VacuumCleaner, List<CellStack>> vacuumCleanersPaths = new Dictionary<VacuumCleaner, List<CellStack>>();

        VacuumCleanerPathFinder vacuumCleanerPathFinder;
        VacuumCleanerActivationPresentationHandler presentationHandler;

        public VacuumCleanerActivationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            vacuumCleanerPathFinder = new VacuumCleanerPathFinder(gameplayController.GameBoard());
            presentationHandler = gameplayController.GetPresentationHandler<VacuumCleanerActivationPresentationHandler>();

            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTileOnTop<VacuumCleaner>(cellStack))
                    notActivatedVacuumCleaners.Add(TopTile(cellStack) as VacuumCleaner);
        }

        public override void Update(float dt)
        {
            CheckForFilledVacuumCleaners();

            ProcessPendingVacuumCleaners();
        }

        void CheckForFilledVacuumCleaners()
        {
            for (int i = notActivatedVacuumCleaners.Count - 1; i >= 0; --i)
            {
                var cleaner = notActivatedVacuumCleaners[i];
                if (cleaner.IsFilled())
                {
                    AddToPending(cleaner);
                    notActivatedVacuumCleaners.RemoveAt(i);
                }
            }
        }

        private void AddToPending(VacuumCleaner cleaner)
        {
            pendingVacuumCleaners.Add(cleaner);

            vacuumCleanersPaths[cleaner] = vacuumCleanerPathFinder.FindFor(cleaner);
        }

        void ProcessPendingVacuumCleaners()
        {
            for (int i = pendingVacuumCleaners.Count - 1; i >= 0; --i)
            {
                var cleaner = pendingVacuumCleaners[i];
                if(IsReadyToActivate(cleaner))
                {
                    pendingVacuumCleaners.RemoveAt(i);
                    Activate(cleaner);
                }
            }

        }

        private void Activate(VacuumCleaner cleaner)
        {
            var path = vacuumCleanersPaths[cleaner];

            FullyLock<VacuumCleanerActivationKeyType>(cleaner.Parent().Parent());
            foreach (var cellStack in path)
                FullyLock<VacuumCleanerActivationKeyType>(cellStack);

            ApplyHitToCellStack(cleaner.Parent().Parent());
            presentationHandler.Activate(cleaner, path, onCellStackReached: ApplyHitToCellStack , onCompleted: ()=> TearDownMovement(cleaner, path));
            vacuumCleanersPaths.Remove(cleaner);
        }

        void ApplyHitToCellStack(CellStack cellStack)
        {
            TryUnlock(cellStack.CurrentTileStack());
            GetFrameData<VacuumCleanerHitData>().cellStacks.Add(cellStack);
        }

        private void TearDownMovement(VacuumCleaner cleaner, List<CellStack> path)
        {
            FullyUnlock(cleaner.Parent().Parent());
            foreach (var cellStack in path)
                FullyUnlock(cellStack);

            var cleanerTileStack = cleaner.Parent();
            cleanerTileStack.Destroy();
        }

        private bool IsReadyToActivate(VacuumCleaner cleaner)
        {
            if (IsFullyFree(cleaner.Parent().Parent()) == false)
                return false;

            foreach (var cellStack in vacuumCleanersPaths[cleaner])
                if (IsFullyFree(cellStack) == false)
                    return false;

            return true;
        }
    }
}