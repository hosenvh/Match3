using Match3.Game.Gameplay.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Match3.Presentation.Gameplay.Core;
using Match3.Game.Gameplay;
using Match3.Foundation.Unity.ObjectPooling;
using PandasCanPlay.HexaWord.Utility;

namespace Match3.Presentation.Gameplay.BorderPresentation
{

    // TODO : Refactor and redesign this FUCKING SHIT.
    public abstract class BorderPresentationHandler : MonoBehaviour
    {
        public enum BorderType { Side_N, Side_S, Side_E, Side_W, 
            InnerConer_NE, InnerConer_NW, InnerConer_SE, InnerConer_SW,
            OuterConer_NE, OuterConer_NW, OuterConer_SE, OuterConer_SW, 
            CrossCorner_NW_To_SE, CrossCorner_NE_To_SW
        }

        [System.Serializable]
        public class BorderSpriteInfo
        {
            public BorderType borderType;
            public Sprite sprite;
            public float rotationOffset;
            public bool flipX;
            public bool flipY;
        }


        [ArrayElementTitle(nameof(BorderSpriteInfo.borderType))]
        public BorderSpriteInfo[] borderSpriteInfos;

        public Image borderImagePrefab;

        protected Grid<BorderInfo> borderInfos;
        protected LeftToRightTopDownGridIterator<BorderInfo> borderInfosIterator;

        protected Dictionary<BorderInfo, Image> borderImages = new Dictionary<BorderInfo, Image>();


        protected IGameplayController gameplayController;
        protected GameBoard gameBoard;
        protected CellStackBoard cellStackBoard;
        //protected LeftToRightTopDownGridIterator<CellStack> cellStackBoardIterator;

        protected UnityComponentObjectPool<Image> imagePool;

        Dictionary<BorderType, BorderSpriteInfo> borderSpriteInfoMap = new Dictionary<BorderType, BorderSpriteInfo>();

        protected float tempValue1;
        protected float tempValue2;
        protected float  cellSize = BoardPresenterNew.CellSize;

        public void Setup(IGameplayController gameplayController)
        {
            this.gameplayController = gameplayController;
            this.gameBoard = gameplayController.GameBoard();
            this.cellStackBoard = gameBoard.CellStackBoard();
            borderInfos = new Grid<BorderInfo>(cellStackBoard.Width() + 1, cellStackBoard.Height() + 1);

            borderInfosIterator = new LeftToRightTopDownGridIterator<BorderInfo>(borderInfos);
            //cellStackBoardIterator = new LeftToRightTopDownGridIterator<CellStack>(cellStackBoard);


            foreach (var element in borderInfosIterator)
                borderInfos[element.x, element.y] = new BorderInfo();

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                SetupBoarderInfo(cellStack.Position().x, cellStack.Position().y, cellStack);


            foreach (var info in borderSpriteInfos)
                borderSpriteInfoMap.Add(info.borderType, info);

            imagePool = new UnityComponentObjectPool<Image>();
            imagePool.SetComponentPrefab(borderImagePrefab);
            imagePool.SetPoolTransform(this.transform);


            tempValue1 = cellSize * (cellStackBoard.Width() / 2f);
            tempValue2 = cellSize * (cellStackBoard.Height() / 2f);

            SetupBoarders();

            InternalSetup();
        }

        protected virtual void InternalSetup() { }

        protected abstract void SetupBoarderInfo(int x, int y, CellStack value);

        protected void SetupBoarders()
        {
            foreach (var element in borderInfosIterator)
                SetupBorderFor(element.x, element.y, element.value);
        }

        // TODO: Refactor this shit.
        protected void SetupBorderFor(int x, int y, BorderInfo borderInfo)
        {
            var fillCount = borderInfo.FillCount();
            if (fillCount == 0 || fillCount == 4)
                return;

            var position = new Vector3(x * cellSize - tempValue1, -y * cellSize + tempValue2);

            switch (fillCount)
            {

                case 1:
                    if (borderInfo.SWFilled)
                        CreateBorder(SpriteInfoFor(BorderType.OuterConer_NE), position, borderInfo);
                    else if (borderInfo.SEFilled)
                        CreateBorder(SpriteInfoFor(BorderType.OuterConer_NW), position, borderInfo);
                    else if (borderInfo.NEFilled)
                        CreateBorder(SpriteInfoFor(BorderType.OuterConer_SW), position, borderInfo);
                    else if (borderInfo.NWFilled)
                        CreateBorder(SpriteInfoFor(BorderType.OuterConer_SE), position, borderInfo);
                    break;

                case 2:
                    if (borderInfo.SEFilled && borderInfo.SWFilled)
                    {
                        CreateBorder(SpriteInfoFor(BorderType.Side_N), position, borderInfo);
                    }
                    else if (borderInfo.NEFilled && borderInfo.NWFilled)
                    {
                        CreateBorder(SpriteInfoFor(BorderType.Side_S), position, borderInfo);
                    }
                    else if (borderInfo.NEFilled && borderInfo.SEFilled)
                    {
                        CreateBorder(SpriteInfoFor(BorderType.Side_W), position, borderInfo);
                    }
                    else if (borderInfo.NWFilled && borderInfo.SWFilled)
                    {
                        CreateBorder(SpriteInfoFor(BorderType.Side_E), position, borderInfo);
                    }
                    else if (borderInfo.NWFilled && borderInfo.SEFilled)
                    {
                        CreateBorder(SpriteInfoFor(BorderType.CrossCorner_NW_To_SE), position, borderInfo);
                    }
                    else if (borderInfo.NEFilled && borderInfo.SWFilled)
                    {
                        CreateBorder(SpriteInfoFor(BorderType.CrossCorner_NE_To_SW), position, borderInfo);
                    }
                    break;

                case 3:
                    if (!borderInfo.SWFilled)
                        CreateBorder(SpriteInfoFor(BorderType.InnerConer_SW), position, borderInfo);
                    else if (!borderInfo.SEFilled)
                        CreateBorder(SpriteInfoFor(BorderType.InnerConer_SE), position, borderInfo);
                    else if (!borderInfo.NEFilled)
                        CreateBorder(SpriteInfoFor(BorderType.InnerConer_NE), position, borderInfo);
                    else if (!borderInfo.NWFilled)
                        CreateBorder(SpriteInfoFor(BorderType.InnerConer_NW), position, borderInfo);
                    break;

            }
        }

        BorderSpriteInfo SpriteInfoFor(BorderType type)
        {
            return borderSpriteInfoMap[type];
        }

        private void CreateBorder(BorderSpriteInfo spriteInfo, Vector3 position, BorderInfo borderInfo)
        {
            var borderImage = imagePool.Acquire();

            borderImage.transform.localPosition = position;
            var rotation = borderImage.transform.localEulerAngles;
            rotation.z =  spriteInfo.rotationOffset;
            borderImage.transform.localEulerAngles = rotation;
            borderImage.sprite = spriteInfo.sprite;

            var scale = borderImage.transform.localScale;
            scale.x *= spriteInfo.flipX ? -1 : 1;
            scale.y *= spriteInfo.flipY ? -1 : 1;
            borderImage.transform.localScale = scale;

            borderImages.Add(borderInfo, borderImage);
        }

    }
}