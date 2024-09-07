using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.DestructionManagement
{

    public interface VacuumCleanerTargetGatheringPresentationHandler : PresentationHandler
    {
        void Gather(Tile tile, VacuumCleaner vacuumCleaner, Action<Tile> onDetached, Action onGathered);
    }

   
    public class VacuumCleanerTargetGatheringDestructionHandler : DestructionHandler
    {
        List<VacuumCleaner> sortedVacuumCleaners = new List<VacuumCleaner>();
        HashSet<TileColor> targetColors = new HashSet<TileColor>();
        VacuumCleanerTargetGatheringPresentationHandler presentationHandler;
        Dictionary<VacuumCleaner, int> pendings = new Dictionary<VacuumCleaner, int>();

        public void Initialize(GameplayController gpc)
        {
            presentationHandler = gpc.GetPresentationHandler<VacuumCleanerTargetGatheringPresentationHandler>();

            foreach (var cellStack in gpc.GameBoard().ArrbitrayCellStackArray())
                if (QueryUtilities.HasTileOnTop<VacuumCleaner>(cellStack))
                    sortedVacuumCleaners.Add(cellStack.CurrentTileStack().Top() as VacuumCleaner);

            sortedVacuumCleaners.Sort((a, b) => a.priority.CompareTo(b.priority));

            foreach (var vacuumCleaner in sortedVacuumCleaners)
            {
                targetColors.Add(vacuumCleaner.targetColor);
                pendings[vacuumCleaner] = 0;
            }
        }

        public bool DoesAccept(Tile tile)
        {
            return sortedVacuumCleaners.Count > 0
                    && tile is ColoredBead coloredBead
                    && coloredBead.IsClean()
                    && targetColors.Contains(tile.GetComponent<TileColorComponent>().color);
        }

        public bool DoesAccept(Cell cell)
        {
            return false;
        }

        public void Destroy(Tile tile, Action<Tile> onCompleted)
        {
            var vacuumCleaner = FindTheHighestPriorityNotFilledVacuumCleaner(ColorOf(tile));

            IncreasePendingOf(vacuumCleaner);
            presentationHandler.Gather(
                tile,
                vacuumCleaner,
                onDetached: onCompleted, 
                onGathered: () => IncreaseFillOf(vacuumCleaner));
        }

        private TileColor ColorOf(Tile tile)
        {
            return tile.GetComponent<TileColorComponent>().color;
        }

        private void IncreasePendingOf(VacuumCleaner vacuumCleaner)
        {
            pendings[vacuumCleaner]++;
            if(IsPotentialyFilled(vacuumCleaner))
            {
                sortedVacuumCleaners.Remove(vacuumCleaner);
                UpdateTargetColors();
            }

        }

        private bool IsPotentialyFilled(VacuumCleaner vacuumCleaner)
        {
            return vacuumCleaner.CurrentFill() + pendings[vacuumCleaner] >= vacuumCleaner.targetNumber;
        }

        private void IncreaseFillOf(VacuumCleaner vacuumCleaner)
        {
            vacuumCleaner.IncreaseFill(1);
            pendings[vacuumCleaner]--;
        }

        private void UpdateTargetColors()
        {
            targetColors.Clear();
            foreach (var vacuumCleaner in sortedVacuumCleaners)
                targetColors.Add(vacuumCleaner.targetColor);  
        }

        private VacuumCleaner FindTheHighestPriorityNotFilledVacuumCleaner(TileColor tileColor)
        {
            // NOTE: Based on the logic it suffices to return the first element in sortedVacuumCleaners
            foreach (var cleaner in sortedVacuumCleaners)
                if (IsPotentialyFilled(cleaner) == false && cleaner.targetColor == tileColor)
                    return cleaner;

            return null;
        }

        private bool IsNotPotentialFilled(VacuumCleaner cleaner)
        {
            throw new NotImplementedException();
        }

        public void Destroy(Cell cell, Action<Cell> onCompleted)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            
        }

        public bool DoesAccept(HitableCellAttachment attachment)
        {
            return false;
        }

        public void Destroy(HitableCellAttachment attachment, Action<HitableCellAttachment> onCompleted)
        {
            throw new NotImplementedException();
        }
    }
}
