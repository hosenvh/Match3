using Match3.Game.Gameplay;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.BorderPresentation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.Gameplay.Core
{
    public struct BoardDimensions
    {
        public Vector2 tileSize;
        public Vector2 origin;
        public Vector2 boardSize;

        public BoardDimensions(Vector2 tileSize, Vector2 origin, Vector2 boardSize)
        {
            this.tileSize = tileSize;
            this.origin = origin;
            this.boardSize = boardSize;
        }

        public Vector2 LogicalPosToPresentaionalPos(Vector2 pos)
        {
            var newPos = pos;
            newPos.y = -pos.y;
            newPos.Scale(tileSize);
            newPos += origin;

            return newPos;
        }

        //public Vector2 PresentaionalPosToLogicalPos(Vector2 pos)
        //{
        //    return Vector2.Scale(Vector2.Scale(pos, new Vector2(-1,1)),  + origin;
        //}
    }

    public class TileStackPresenterContainer
    {
        List<TileStackPresenter> tileStackPresenters = new List<TileStackPresenter>();
        BoardDimensions boardDimensions;

        public TileStackPresenterContainer(BoardDimensions boardDimensions)
        {
            this.boardDimensions = boardDimensions;
        }

        public void Add(TileStackPresenter presenter)
        {
            tileStackPresenters.Add(presenter);
            presenter.UpdatePosition(boardDimensions);
        }

        public void Remove(TileStackPresenter presenter)
        {
            tileStackPresenters.Remove(presenter);
        }

        public void Update()
        {
            foreach (var p in tileStackPresenters)
                p.UpdatePosition(boardDimensions);
        }
    }

    
    public class BoardPresenterNew : Base
    {
        public const float Boat_Swap_MoveDuration = 0.2f; // todo
        public const float CellSize = 64;

        public TileStackPresenterGenerator tileStackPresenterGenerator;
        public CellStackPresenterGenerator cellStackPresenterGenerator;

        public TileStackPresenter tileStackPresenterPrefab;
        public CellStackPresenter cellStackPresenterPrefab;

        public List<RectTransform> cellCotainers;

        public RectTransform postTileAttachmentContainer;
        public RectTransform preTileAttachmentContainer;


        public List<BorderPresentationHandler> borderPresentationHandlers;

        [SerializeField]
        private RectTransform cellStackContainer = null, tileStackContainer = null;

        public TilePresenterFactory tilePresenterFactory;
        public RectTransform containerAndPresenters;

        BoardDimensions boardDimensions;
        public static float BoatOrSwapSpeed;
        public int UsedExplosiveCount { get; private set; }


        Grid<CellStackPresenter> cellPresentersGrid;
        IGameplayController gameplayController;

        CellStackBoard cellStackBoard;

        TileStackPresenterContainer tileStackPresenterContainer;


        public BoardPresenterNew Setup(IGameplayController gameplayController)
        {
            this.gameplayController = gameplayController;

            this.cellStackBoard = gameplayController.GameBoard().CellStackBoard();
            cellPresentersGrid = new Grid<CellStackPresenter>(cellStackBoard.Width(), cellStackBoard.Height());
            SetupBoardDimenstions();
            tileStackPresenterContainer = new TileStackPresenterContainer(boardDimensions);
            SetupBoardCellsAndTiles();

            foreach (var handler in borderPresentationHandlers)
                handler.Setup(gameplayController);
            tileStackPresenterGenerator.Setup(this);
            cellStackPresenterGenerator.Setup(this);

            return this;
        }

        private void SetupBoardDimenstions()
        {
            Vector2 origin =
            new Vector2(
                -CellSize * (cellStackBoard.Width() / 2f - .5f),
                CellSize * (cellStackBoard.Height() / 2f - .5f));

            Vector2 boardSize =
              new Vector2(
                cellStackBoard.Width() * CellSize,
                cellStackBoard.Height() * CellSize);


            boardDimensions = new BoardDimensions(new Vector2(CellSize, CellSize), origin, boardSize);
        }

        void SetupBoardCellsAndTiles()
        {
            tilePresenterFactory.Setup();
            foreach (var element in new LeftToRightTopDownGridIterator<CellStack>(cellStackBoard))
            {
                SetupCellAndTileFor(element.value);
            }  
        }

        public void SetupCellAndTileFor(CellStack cellStack)
        {
            var logicalPos = cellStack.Position();
            Vector3 position = boardDimensions.LogicalPosToPresentaionalPos(logicalPos);
            CreateCellStackPresenter(logicalPos.x, logicalPos.y, cellStack, position);
            TryCreateTileStackPresenterFor(cellStack);
        }

        private void CreateCellStackPresenter(int x, int y, CellStack value, Vector3 position)
        {
            //if (value.Top() is EmptyCell)
            //    return;
            var cellStack = Instantiate(cellStackPresenterPrefab, this.cellStackContainer, false);

            cellStack.transform.localPosition = position;
            cellStack.Setup(value, ContainerForCellLevel, ContainerForCellAttachment);

            cellPresentersGrid[x, y] = cellStack;
        }

        RectTransform ContainerForCellLevel(int level)
        {
            return cellCotainers[Mathf.Min(level, cellCotainers.Count-1)];
        }

        // TODO: Make this configurable.
        RectTransform ContainerForCellAttachment(CellAttachment attachment)
        {
            if (attachment is Wall || attachment is Rope)
                return postTileAttachmentContainer;
            else
                return preTileAttachmentContainer;
        }

        private void TryCreateTileStackPresenterFor(CellStack value)
        {
            if (value.HasTileStack())
                CreateTileStackPresenterFor(value.CurrentTileStack());
        }

        public void CreateTileStackPresenterFor(TileStack tileStack)
        {
            var presenter = Instantiate(tileStackPresenterPrefab, this.tileStackContainer, false);

            presenter.Setup(tileStack, ref tileStackPresenterContainer, tilePresenterFactory);
        }


        public void CreateTilePresenterFor(Tile tile, TileStack tileStack)
        {
            var presenter = tileStack.GetComponent<TileStackPresenter>();
            presenter.SetupTilePresentationFor(tile, tilePresenterFactory);
        }


        public void CreateCellPresenterFor(Cell cell, CellStack cellStack)
        {
            var presenter = cellStack.GetComponent<CellStackPresenter>();
            presenter.SetupCellPresentationFor(cell, cellStack.Stack().Count-1);
        }


        public T GetGameplaySystem<T>() where T: GameplaySystem
        {
            return gameplayController.GetSystem<T>();
        }

        public IGameplayController GameplayController()
        {
            return gameplayController;
        }

        public Grid<CellStackPresenter> CellPresentersGrid()
        {
            return cellPresentersGrid;
        }

        public BoardDimensions BoardDimensions()
        {
            return boardDimensions;
        }

        private void Update()
        {
            if(tileStackPresenterContainer != null)
                tileStackPresenterContainer.Update();
        }

    }
}
