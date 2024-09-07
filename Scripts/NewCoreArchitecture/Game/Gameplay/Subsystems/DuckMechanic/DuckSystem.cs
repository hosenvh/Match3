using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;

namespace Match3.Game.Gameplay.SubSystems.DuckMechanic
{
    public abstract class DuckSystem : GameplaySystem
    {
        private List<RiverCell> potentialRiverCells = new List<RiverCell>(32);
        private List<RiverCell> availableRiverCells = new List<RiverCell>(32);
        private UserMovementData userMovementData;

        protected static List<Duck> ducks;
        protected CellStackBoard cellStackBoard;
        protected TileFactory tileFactory;
        protected const int GENERATION_DEFAULT_TURNS_TO_WAIT_COUNT = 2;
        protected static int totalDucksCountInBoard;
        protected static int maxDucksCountInLevel;
        protected static int maxDuckCountInBoard;
        protected static int totalRemainingDucksCountToGenerate;
        public static IEnumerable<Type> childTilesTypes { get; private set; }
        protected static List<GenerationData> scheduledGenerationData = new List<GenerationData>(4);
        private Type[] targetExceptionTypes = { typeof(TableClothMainTile) };
        
        public DuckSystem (GameplayController gameplayController) : base (gameplayController)
        {
            cellStackBoard = gameplayController.GameBoard().CellStackBoard();
            CellStack[] cellStacks = gameplayController.GameBoard().ArrbitrayCellStackArray();
            potentialRiverCells = QueryUtilities.ExtractCellsOnTop<RiverCell>(cellStacks, IsPotentialRiverCell);
            userMovementData = GetFrameData<UserMovementData>();
            tileFactory = ServiceLocator.Find<TileFactory>();
        }

        public static void SetMaxDucksCount (int inLevelMaxCount, int inBoardMaxCount)
        {
            maxDucksCountInLevel = inLevelMaxCount;
            maxDuckCountInBoard = inBoardMaxCount;
        }

        public static void SetChildTilesTypes (List<string> childTilesItems)
        {
            List<Type> childTiles = new List<Type>(childTilesItems.Count);
            foreach (string type in childTilesItems)
                childTiles.Add(Type.GetType(type));

            childTilesTypes = childTiles;
        }

        protected bool IsUserMoved()
        {
            return userMovementData.moves > 0;
        }

        private bool IsPotentialRiverCell (RiverCell riverCell)
        {
            return 
                riverCell.Parent().HasAttachment<LilyPad>() == false && 
                riverCell.Parent().HasAttachment<LilyPadBud>() == false;
        }

        protected List<RiverCell> ExtractAvailableRiverCells (Predicate<RiverCell> isAvailable)
        {
            availableRiverCells.Clear();
            foreach (var riverCell in potentialRiverCells)
                if (isAvailable(riverCell))
                    availableRiverCells.Add(riverCell);

            RemoveTargetExceptions(ref availableRiverCells);
            return availableRiverCells;
        }

        private void RemoveTargetExceptions(ref List<RiverCell> targets)
        {
            targets.RemoveAll(target => 
                DoesRiverCellHaveParent(target) &&
                DoesParentHaveTileStack(target) &&
                !IsParentTileStackDepleted(target) &&
                IsTopTileException(target.Parent().CurrentTileStack().Top())
            );

            bool DoesRiverCellHaveParent(RiverCell riverCell) => riverCell.Parent() != null;

            bool DoesParentHaveTileStack(RiverCell riverCell) => riverCell.Parent().HasTileStack();

            bool IsParentTileStackDepleted(RiverCell riverCell) => riverCell.Parent().CurrentTileStack().IsDepleted();

            bool IsTopTileException(Tile tile)
            {
                if (IsSelfExceptionType() || IsMasterExceptionType())
                    return true;

                return false;

                bool IsSelfExceptionType()
                {
                    foreach (var exceptionType in targetExceptionTypes)
                        if (tile.GetType() == exceptionType)
                            return true;
                    
                    return false;
                }

                bool IsMasterExceptionType()
                {
                    if (tile is SlaveTile slaveTile)
                        foreach (var exceptionType in targetExceptionTypes)
                            if (slaveTile.Master().GetType() == exceptionType)
                                return true;
                    
                    return false;
                }
            }
        }

        protected void TryScheduleGeneration (int waitTime, int generationCount)
        {
            if (totalDucksCountInBoard >= maxDuckCountInBoard || totalRemainingDucksCountToGenerate <= 0)
                return;

            var avaiableGenerations = Math.Min(maxDuckCountInBoard - totalDucksCountInBoard, totalRemainingDucksCountToGenerate);
            generationCount = Math.Min(generationCount, avaiableGenerations);
            scheduledGenerationData.Add(new GenerationData(waitTime, generationCount));
        }
    }
}