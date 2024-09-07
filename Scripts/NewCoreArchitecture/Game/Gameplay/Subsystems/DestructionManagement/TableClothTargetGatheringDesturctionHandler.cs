using System;
using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.DestructionManagement
{

    public interface TableClothTargetGatheringPresentationHandler : PresentationHandler
    {
        void Gather(Tile tile, TableClothMainTile tableCloth, TableClothMainTile.TargetHandler target, Action<Tile> onDetached, Action onGathered);
    }

   
    public class TableClothTargetGatheringDestructionHandler : DestructionHandler
    {
        List<TableClothMainTile.TargetHandler> tableClothTargets = new List<TableClothMainTile.TargetHandler>();

        Dictionary<TableClothMainTile.TargetHandler, TableClothMainTile> targetOwnershipMap = new Dictionary<TableClothMainTile.TargetHandler, TableClothMainTile>();

        HashSet<GoalTargetType> targets = new HashSet<GoalTargetType>();
        TableClothTargetGatheringPresentationHandler presentationHandler;
        Dictionary<TableClothMainTile.TargetHandler, int> pendings = new Dictionary<TableClothMainTile.TargetHandler, int>();

        public void Initialize(GameplayController gpc)
        {
            presentationHandler = gpc.GetPresentationHandler<TableClothTargetGatheringPresentationHandler>();

            foreach (var cellStack in gpc.GameBoard().ArrbitrayCellStackArray())
                if (QueryUtilities.HasTileOnTop<TableClothMainTile>(cellStack))
                {
                    var tableCloth = cellStack.CurrentTileStack().Top() as TableClothMainTile;

                    AddTarget(tableCloth.firstTarget, tableCloth);
                    if(tableCloth.secondTarget.isActive)
                        AddTarget(tableCloth.secondTarget, tableCloth);
                }

            foreach (var tableClothTarget in tableClothTargets)
            {
                pendings[tableClothTarget] = 0;
            }
        }

        private void AddTarget(TableClothMainTile.TargetHandler target, TableClothMainTile owner)
        {
            tableClothTargets.Add(target);
            targets.Add(target.goalType);
            targetOwnershipMap[target] = owner;
        }

        public bool DoesAccept(Tile tile)
        {
            return tableClothTargets.Count > 0
                    && tile is GoalObject
                    && targets.Find(target => target.Includes((GoalObject) tile)) != null;
        }

        public bool DoesAccept(Cell cell)
        {
            return false;
        }

        public void Destroy(Tile tile, Action<Tile> onCompleted)
        {
            var target = FindANotFilledTableClothTarget(tile);

            IncreasePendingOf(target);
            presentationHandler.Gather(
                tile,
                targetOwnershipMap[target],
                target,
                onDetached: onCompleted, 
                onGathered: () => IncreaseFillOf(target));
        }

        private void IncreasePendingOf(TableClothMainTile.TargetHandler target)
        {
            pendings[target]++;
            if(IsPotentialyFilled(target))
            {
                tableClothTargets.Remove(target);
                UpdateTargetColors();
            }

        }

        private bool IsPotentialyFilled(TableClothMainTile.TargetHandler target)
        {
            return target.CurrentFill() + pendings[target] >= target.targetNumber;
        }

        private void IncreaseFillOf(TableClothMainTile.TargetHandler target)
        {
            target.IncreaseFill(1);
            pendings[target]--;
        }

        private void UpdateTargetColors()
        {
            targets.Clear();
            foreach (var targetHandler in tableClothTargets)
                targets.Add(targetHandler.goalType);
        }

        private TableClothMainTile.TargetHandler FindANotFilledTableClothTarget(Tile tile)
        {
            foreach (var target in tableClothTargets)
                if (IsPotentialyFilled(target) == false && target.goalType.Includes((GoalObject) tile))
                    return target;

            return null;
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
