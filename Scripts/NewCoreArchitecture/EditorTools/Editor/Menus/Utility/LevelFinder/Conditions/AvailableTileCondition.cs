using System;
using System.Collections.Generic;
using Match3.EditorTools.Editor.Base;
using Match3.EditorTools.Editor.Drawers;
using Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base;
using Match3.EditorTools.Editor.Utility;
using Match3.Foundation.Base.DataStructures;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Initialization;
using Match3.Game.Gameplay.Tiles;


namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions
{
    public class AvailableTileCondition : BoardFinderCondition
    {
        private readonly Type tileType;
        private readonly Bound<int> tileCountBound;

        public AvailableTileCondition(Type tileType, Bound<int> tileCountBound)
        {
            this.tileType = tileType;
            this.tileCountBound = tileCountBound;

            EditorBoardCreationsServicesInitializer.InitializeServices();
        }

        public bool IsSatisfied(BoardConfig boardConfig)
        {
            GameBoard gameBoard = CreateGameBoardFromConfig(boardConfig);
            List<Tile> allTiles = GetAllTilesInBoard(gameBoard);
            int availableTilesCount = GetTilesOfTypeCount(targetTileType: tileType, allTiles);

            return tileCountBound.Contains(availableTilesCount);
        }

        private GameBoard CreateGameBoardFromConfig(BoardConfig boardConfig)
        {
            return new GameBoardCreator().CreateFrom(boardConfig);
        }

        private List<Tile> GetAllTilesInBoard(GameBoard gameBoard)
        {
            List<Tile> allTiles = new List<Tile>();
            CellStack[] allCellStacks = gameBoard.ArrbitrayCellStackArray();
            foreach (CellStack cellStack in allCellStacks)
            {
                if (cellStack.HasTileStack() == false)
                    continue;
                var tileStack = cellStack.CurrentTileStack();
                Stack<Tile> tiles = tileStack.Stack();
                allTiles.AddRange(tiles);
            }

            return allTiles;
        }

        private int GetTilesOfTypeCount(Type targetTileType, List<Tile> allTiles)
        {
            return allTiles.FindAll(tile => tile.GetType() == targetTileType).Count;
        }
    }

    public class AvailableTileConditionDrawer : BoardFinderConditionDrawer<AvailableTileCondition>
    {
        private Type tileType = typeof(ColoredBead);
        private Bound<int> tileCountBound = new Bound<int>();
        private TypeDropdownDrawer typeAttributeDrawer = new TypeDropdownDrawer(targetTypes: new List<Type> {typeof(Tile)}, excludingTypes: new List<Type>(), includeAbstracts: false, showPartialNames: true);

        protected override void DrawConditionsInternal()
        {
            DrawTileTypeCondition();
            DrawTileCountCondition();
        }

        private void DrawTileTypeCondition()
        {
            EditorUtilities.InsertTypeSelectionDropDown(ref tileType, label: "Tile Type:", typeAttributeDrawer, labelsWidth: 70);
        }

        private void DrawTileCountCondition()
        {
            EditorUtilities.InsertIntBoundField(ref tileCountBound, label: "Tile Count");
        }

        protected override AvailableTileCondition GetSelectedCondition()
        {
            return new AvailableTileCondition(tileType, tileCountBound);
        }
    }
}