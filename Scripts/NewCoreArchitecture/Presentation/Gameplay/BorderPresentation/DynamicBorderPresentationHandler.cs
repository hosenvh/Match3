using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay.BorderPresentation
{
    public abstract class DynamicBorderPresentationHandler : BorderPresentationHandler
    {
        HashSet<Vector2Int> dirtyBoarders = new HashSet<Vector2Int>();

        protected override void InternalSetup()
        {
            base.InternalSetup();
            dirtyBoarders.Clear();
        }


        protected override void SetupBoarderInfo(int x, int y, CellStack cellStack)
        {
            bool hasBorder = ShouldConsiderBordersOf(cellStack);

            SetFillSE(x, y, hasBorder);
            SetFillNE(x, y+1, hasBorder);
            SetFillSW(x +1, y, hasBorder);
            SetFillNW(x+1, y+1, hasBorder);
            
        }

        private void SetFillNW(int x, int y, bool value)
        {
            CheckForDirty(x, y, borderInfos[x, y].NWFilled, value);
            borderInfos[x, y].NWFilled = value;
        }

        private void SetFillSW(int x, int y, bool value)
        {
            CheckForDirty(x, y, borderInfos[x, y].SWFilled, value);
            borderInfos[x, y].SWFilled = value;
        }

        private void SetFillNE(int x, int y, bool value)
        {
            CheckForDirty(x, y, borderInfos[x, y].NEFilled, value);
            borderInfos[x, y].NEFilled = value;
        }

        private void SetFillSE(int x, int y, bool value)
        {
            CheckForDirty(x, y, borderInfos[x, y].SEFilled, value);
            borderInfos[x, y].SEFilled = value;
        }

        void CheckForDirty(int x, int y, bool currentValue, bool nextValue)
        {
            if (currentValue != nextValue)
                dirtyBoarders.Add(new Vector2Int(x,y));
        }

        protected abstract bool ShouldConsiderBordersOf(CellStack cellStack);


        private void Update()
        {
            if (dirtyBoarders.Count > 0)
                dirtyBoarders.Clear();

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                SetupBoarderInfo(cellStack.Position().x, cellStack.Position().y, cellStack);


            foreach (var position in dirtyBoarders)
                ResetupBorderIn(position);
        }

        private void ResetupBorderIn(Vector2Int position)
        {
            var borderInfo = borderInfos[position];

            if (borderImages.ContainsKey(borderInfo))
            {
                var image = borderImages[borderInfo];
                borderImages.Remove(borderInfo);

                imagePool.Release(image);
            }

            SetupBorderFor(position.x, position.y, borderInfo);

        }
    }
}