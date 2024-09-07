

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.HitManagement
{

    public struct TileStackHitData
    {
        public readonly TileStack tileStack;
        public readonly HitCause hitCause;
        public readonly HitType hitType;

        public TileStackHitData(TileStack tileStack, HitCause hitCause, HitType hitType)
        {
            this.tileStack = tileStack;
            this.hitCause = hitCause;
            this.hitType = hitType;
        }
    }

    public struct DelayedTileStackHitData
    {
        public readonly float delay;
        public readonly TileStackHitData tileHitData;

        public DelayedTileStackHitData(float delay, TileStackHitData tileHitData)
        {
            this.delay = delay;
            this.tileHitData = tileHitData;
        }
    }

    public struct CellStackHitData
    {
        public readonly CellStack cellStack;
        public readonly HitType hitType;

        public CellStackHitData(CellStack cellStack, HitType hitType)
        {
            this.cellStack = cellStack;
            this.hitType = hitType;
        }
    }
    public struct DelayedCellStackHitData
    {
        public readonly float delay;
        public readonly CellStackHitData hitData;

        public DelayedCellStackHitData(float delay, CellStackHitData hitData)
        {
            this.delay = delay;
            this.hitData = hitData;
        }
    }


    public struct CellAttachmentHitData
    {
        public readonly HitableCellAttachment cellAttachment;
        //public readonly HitType hitType;

        public CellAttachmentHitData(HitableCellAttachment cellAttachment)
        {
            this.cellAttachment = cellAttachment;
        }
    }

    public struct DelayedCellAttachmentHitData
    {
        public readonly float delay;
        public readonly CellAttachmentHitData hitData;

        public DelayedCellAttachmentHitData(float delay, CellAttachmentHitData hitData)
        {
            this.delay = delay;
            this.hitData = hitData;
        }
    }




    public class GeneratedHitsData : BlackBoardData
    {
        public List<TileStackHitData> tileStacksHitData = new List<TileStackHitData>(64);
        public List<DelayedTileStackHitData> delayedTileStacksHitData = new List<DelayedTileStackHitData>(64);

        public List<CellStackHitData> cellStacksHitData = new List<CellStackHitData>(64);
        public List<DelayedCellStackHitData> delayedCellStacksHitData = new List<DelayedCellStackHitData>(64);

        public List<CellAttachmentHitData> cellAttachmentsHitData = new List<CellAttachmentHitData>(64);
        public List<DelayedCellAttachmentHitData> delayedcellAttachmentsHitData = new List<DelayedCellAttachmentHitData>(64);

        public void Clear()
        {
            tileStacksHitData.Clear();
            cellStacksHitData.Clear();
            cellAttachmentsHitData.Clear();
            delayedCellStacksHitData.Clear();
            delayedTileStacksHitData.Clear();
            delayedcellAttachmentsHitData.Clear();
        }
    }
}