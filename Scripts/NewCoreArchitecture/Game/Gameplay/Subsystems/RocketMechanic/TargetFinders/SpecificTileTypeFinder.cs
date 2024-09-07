using Match3.Game.Gameplay.Core;
using static Match3.Game.Gameplay.QueryUtilities;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using System;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders
{
    public class SpecificTileTypeFinder : TargetFinder
    {
        Type specifiedTileType;
        bool checkTopOnly;
        TypeExclusionChecker exclusionChecker;

        public SpecificTileTypeFinder(Type tileType, bool checkTopOnly , TypeExclusionChecker exclusionChecker)
        {
            this.specifiedTileType = tileType;
            this.checkTopOnly = checkTopOnly;
            this.exclusionChecker = exclusionChecker;
        }

        public void Find(GameBoard gameBoard, ref List<CellStack> targets)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if ( HasSpecificTile(cellStack) && IsFullyFree(cellStack) && exclusionChecker.IsNotExcluded(TopTile(cellStack).GetType()))
                    targets.Add(cellStack);
        }

        private bool HasSpecificTile(CellStack cellStack)
        {
            if (checkTopOnly)
                return HasTileOnTop(cellStack, specifiedTileType);
            else
                return HasTile(cellStack, specifiedTileType);
        }
    }
}