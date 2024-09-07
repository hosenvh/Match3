using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.GameplayConditions;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;

namespace Match3.Game.Gameplay
{
    public class RocketTargetFinder
    {
        struct RocketTargetingData
        {
            public TargetFinder targetFinder;
            public GameplayCondition condition;

            public RocketTargetingData(TargetFinder targetFinder, GameplayCondition condition)
            {
                this.targetFinder = targetFinder;
                this.condition = condition;
            }
        }

        GameplayController gameplayController;
        SortedList<int, List<RocketTargetingData>> rocketTargetingDataBuckets = new SortedList<int, List<RocketTargetingData>>();
        GroupBasedRandomChooser<CellStack> randomChooser = new GroupBasedRandomChooser<CellStack>();
        Type[] targetExceptionTypes = { typeof(TableClothMainTile) };

        public RocketTargetFinder(GameplayController gameplayController)
        {
            this.gameplayController = gameplayController;

            // NOTE: No added targetingData must have "higher" priority than these.
            AddTargetingData(99, new SpecificTileTypeFinder(typeof(ColoredBead), true, new TypeExclusionChecker(new HashSet<Type>())), new AlwaysTrueGamepleplayCondition());
            AddTargetingData(100, new EmptyCellFinder(), new AlwaysTrueGamepleplayCondition());
        }

        public void AddTargetingData(int priority, TargetFinder targetFinder, GameplayCondition condition)
        {
            if (rocketTargetingDataBuckets.ContainsKey(priority) == false)
                rocketTargetingDataBuckets.Add(priority, new List<RocketTargetingData>());

            rocketTargetingDataBuckets[priority].Add(new RocketTargetingData(targetFinder, condition));
        }

        public List<CellStack> FindTargets(int totalNeeded)
        {
            List<CellStack> choosenCellStack = new List<CellStack>();
            randomChooser.Clear();

            int currentlyNeeded = totalNeeded;
            foreach (var bucket in rocketTargetingDataBuckets.Values)
            {
                AddTargetsForBucket(bucket);

                choosenCellStack.AddRange(randomChooser.Choose(currentlyNeeded, choosenCellStack));

                if (choosenCellStack.Count == totalNeeded)
                    break;

                currentlyNeeded = totalNeeded - choosenCellStack.Count;
            }

            return choosenCellStack;
        }

        private void AddTargetsForBucket(List<RocketTargetingData> bucket)
        {
            foreach (var targetingData in bucket)
                if (targetingData.condition.IsSatisfied(gameplayController))
                {
                    var targets = new List<CellStack>();
                    targetingData.targetFinder.Find(gameplayController.GameBoard(), ref targets);
                    RemoveTargetExceptions(ref targets);
                    randomChooser.AddGroup(targets);
                }
        }

        private void RemoveTargetExceptions(ref List<CellStack> targets)
        {
            targets.RemoveAll(target => 
                target.HasTileStack() &&
                !target.CurrentTileStack().IsDepleted() &&
                IsTopTileException(target.CurrentTileStack().Top())
            );

            bool IsTopTileException(Tile tile)
            {
                if (IsSelfExceptionType() || IsSlaveOfExceptionType())
                    return true;

                return false;

                bool IsSelfExceptionType()
                {
                    foreach (var exceptionType in targetExceptionTypes)
                        if (tile.GetType() == exceptionType)
                            return true;
                    
                    return false;
                }

                bool IsSlaveOfExceptionType()
                {
                    if (tile is SlaveTile slaveTile)
                        foreach (var exceptionType in targetExceptionTypes)
                            if (slaveTile.Master().GetType() == exceptionType)
                                return true;
                    
                    return false;
                }
            }
        }
    }

}