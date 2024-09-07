using Match3.Game.Gameplay.Core;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Cells;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Match3.Game.Gameplay
{
    // TODO: Unify item generation in gameplay.
    public class CreationController
    {
        private EventManager eventManager;
        private GeneratedObjectsData generatedObjectsData;
        private TileFactory tileFactory;
        private CellFactory cellFactory;
        CellStackBoard cellStackBoard;



        public CreationController(GeneratedObjectsData generatedObjectsData, TileFactory tileFactory, CellFactory cellFactory, CellStackBoard cellStackBoard)
        {
            this.generatedObjectsData = generatedObjectsData;
            this.tileFactory = tileFactory;
            this.cellFactory = cellFactory;
            this.cellStackBoard = cellStackBoard;
            this.eventManager = ServiceLocator.Find<EventManager>();
        }

        public void ReplaceTileInBoard(Tile tile, CellStack cellStack)
        {
            if (cellStack.HasTileStack())
            {
                cellStack.CurrentTileStack().Destroy();
            }

            var tileStack = tileFactory.CreateTileStack();
            tileStack.Push(tile);

            cellStack.SetCurrnetTileStack(tileStack);
            tileStack.SetPosition(cellStack.Position());

            eventManager.Propagate(new TileStackGeneratedEvent(tileStack), this);
        }

        public void PlaceTileInBoard(Tile tile, CellStack cellStack)
        {

            if (cellStack.HasTileStack() == false)
            {
                cellStack.SetCurrnetTileStack(tileFactory.CreateTileStack());
                eventManager.Propagate(new TileStackGeneratedEvent(cellStack.CurrentTileStack()), this);
            }

            var tileStack = cellStack.CurrentTileStack();
            tileStack.Push(tile);
            tileStack.SetPosition(cellStack.Position());
            generatedObjectsData.tiles.Add(tile);

            eventManager.Propagate(new TileGeneratedEvent(tileStack, tile), this);
        }

        public void PlaceCellInBoard(Cell cell, CellStack cellStack)
        {
            cellStack.Push(cell);
            generatedObjectsData.cells.Add(cell);

            eventManager.Propagate(new CellGeneratedEvent(cellStack, cell), this);

            if(cell is CompositeCell compositeCell)
            {
                var pos = cell.Parent().Position();
                var size = compositeCell.Size();

                for (int i = 0; i < size.Witdth; ++i)
                    for (int j = 0; j < size.Height; j++)
                        if (i != 0 || j != 0)
                            PlaceCellInBoard(cellFactory.CreateSlaveCell(compositeCell), cellStackBoard[pos.x + i, pos.y + j]);
            }
        }

        // TODO: Refactor this.
        public void MoveToTop(Cell targetCell)
        {
            var stack = new Stack<Cell>();
            var cellStack = targetCell.Parent();

            while(true)
            {
                var cell = cellStack.Stack().Pop();
                if (cell == targetCell)
                    break;
                else
                    stack.Push(cell);
            }

            foreach (var cell in stack)
                cellStack.Push(cell);

            cellStack.Push(targetCell);
            eventManager.Propagate(new CellReorderedEvent(cellStack, targetCell), this);
        }
    }
}