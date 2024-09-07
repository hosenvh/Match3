using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Matching;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;



namespace Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders
{
    public class SpecificColoredTileTypeFinder : TargetFinder
    {
        Type specifiedTileType;
        TileColor specifiedTileColor;
        bool checkTopOnly;
        TypeExclusionChecker exclusionChecker;

        public SpecificColoredTileTypeFinder(Type tileType, TileColor tileColor, bool checkTopOnly, TypeExclusionChecker exclusionChecker)
        {
            this.specifiedTileType = tileType;
            this.specifiedTileColor = tileColor;
            this.checkTopOnly = checkTopOnly;
            this.exclusionChecker = exclusionChecker;
        }

        public void Find(GameBoard gameBoard, ref List<CellStack> targets)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if (HasSpecifiedTile(cellStack) && IsFullyFree(cellStack) && exclusionChecker.IsNotExcluded(TopTile(cellStack).GetType()))
                    targets.Add(cellStack);
        }

        bool HasSpecifiedTile(CellStack cellStack)
        {
            if (cellStack.HasTileStack() == false || cellStack.CurrentTileStack().IsDepleted())
                return false;

            if (checkTopOnly)
                return IsSpecificTile(TopTile(cellStack));
            else
            {
                foreach (var tile in cellStack.CurrentTileStack().Stack())
                    if (IsSpecificTile(tile))
                        return true;
            }


            return false;
        }

        bool IsSpecificTile(Tile tile)
        {
            return specifiedTileType.IsAssignableFrom(tile.GetType())
                    && tile.GetComponent<TileColorComponent>().color == specifiedTileColor;
        }
    }
}