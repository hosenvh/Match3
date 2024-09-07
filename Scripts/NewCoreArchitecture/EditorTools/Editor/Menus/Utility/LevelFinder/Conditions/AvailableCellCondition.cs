using System;
using System.Collections.Generic;
using Match3.EditorTools.Editor.Base;
using Match3.EditorTools.Editor.Drawers;
using Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base;
using Match3.EditorTools.Editor.Utility;
using Match3.Foundation.Base.DataStructures;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Initialization;
using Match3.Main;


namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions
{
    public class AvailableCellCondition : BoardFinderCondition
    {
        private readonly Type cellType;
        private readonly Bound<int> cellCountBound;

        public AvailableCellCondition(Type cellType, Bound<int> cellCountBound)
        {
            this.cellType = cellType;
            this.cellCountBound = cellCountBound;

            EditorBoardCreationsServicesInitializer.InitializeServices();
        }

        public bool IsSatisfied(BoardConfig boardConfig)
        {
            GameBoard gameBoard = CreateGameBoardFromConfig(boardConfig);
            List<Cell> allCells = GetAllCellsInBoard(gameBoard);
            int availableCellsCount = GetCellsOfTypeCount(targetCellType: cellType, allCells);

            return cellCountBound.Contains(availableCellsCount);
        }

        private GameBoard CreateGameBoardFromConfig(BoardConfig boardConfig)
        {
            return new GameBoardCreator().CreateFrom(boardConfig);
        }

        private List<Cell> GetAllCellsInBoard(GameBoard gameBoard)
        {
            List<Cell> allCells = new List<Cell>();
            CellStack[] allCellStacks = gameBoard.ArrbitrayCellStackArray();
            foreach (CellStack cellStack in allCellStacks)
                allCells.AddRange(cellStack.Stack());

            return allCells;
        }

        private int GetCellsOfTypeCount(Type targetCellType, List<Cell> allCells)
        {
            return allCells.FindAll(cell => cell.GetType() == targetCellType).Count;
        }
    }

    public class AvailableCellConditionDrawer : BoardFinderConditionDrawer<AvailableCellCondition>
    {
        private Type cellType = typeof(EmptyCell);
        private Bound<int> cellCountBound = new Bound<int>();
        private TypeDropdownDrawer typeAttributeDrawer = new TypeDropdownDrawer(targetTypes: new List<Type> {typeof(Cell)}, excludingTypes: new List<Type>(), includeAbstracts: false, showPartialNames: true);

        protected override void DrawConditionsInternal()
        {
            DrawCellTypeCondition();
            DrawCellCountCondition();
        }

        private void DrawCellTypeCondition()
        {
            EditorUtilities.InsertTypeSelectionDropDown(ref cellType, label: "Cell Type:", typeAttributeDrawer, labelsWidth: 70);
        }

        private void DrawCellCountCondition()
        {
            EditorUtilities.InsertIntBoundField(ref cellCountBound, label: "Cell Count");
        }

        protected override AvailableCellCondition GetSelectedCondition()
        {
            return new AvailableCellCondition(cellType, cellCountBound);
        }
    }
}