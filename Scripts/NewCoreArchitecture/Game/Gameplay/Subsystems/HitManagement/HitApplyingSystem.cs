

using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.HitManagement
{
    // TODO: Merge this two
    public interface TileStackHitPresentationHandler : PresentationHandler
    {
        void ExecuteEffectOn(Tile tile, Action onCompleted);
    }
    
    public interface CellStackHitPresentationHandler : PresentationHandler
    {
        void ExecuteEffectOn(CellStack cellStack, Action onCompleted);
    }

    public interface CellAttachmentHitPresentationHandler : PresentationHandler
    {
        void ExecuteEffectOn(HitableCellAttachment cellAttachment, Action onCompleted);
    }

    public class HitApplyingKeyType : KeyType
    { }

    [AfterAttribute(typeof(HitGenerationSystem))]
    public class HitApplyingSystem : GameplaySystem
    {
        GeneratedHitsData generatedHitsData;
        AppliedHitsData appliedHitsData;
        DestructionData destructionData;

        List<DelayedTileStackHitData> scheduledTileStackHits = new List<DelayedTileStackHitData>(64);
        List<DelayedCellStackHitData> scheduledCellStackHits = new List<DelayedCellStackHitData>(64);
        List<DelayedCellAttachmentHitData> scheduledAttachmentHits = new List<DelayedCellAttachmentHitData>(16);

        Stack<int> activatedHitDelays = new Stack<int>();

        TileStackHitPresentationHandler tileHittingPort;
        CellStackHitPresentationHandler cellHittingPort;
        CellAttachmentHitPresentationHandler attachmentHittingPort;

        public HitApplyingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            this.generatedHitsData = GetFrameData<GeneratedHitsData>();
            this.appliedHitsData = GetFrameData<AppliedHitsData>();
            this.destructionData = GetFrameData<DestructionData>();

            tileHittingPort = gameplayController.GetPresentationHandler<TileStackHitPresentationHandler>();
            cellHittingPort = gameplayController.GetPresentationHandler<CellStackHitPresentationHandler>();
            attachmentHittingPort = gameplayController.GetPresentationHandler<CellAttachmentHitPresentationHandler>();
        }

        public override void Update(float dt)
        {
            ProcessScheduleHits(dt);
            ProcessGeneratedHits();
        }

        private void ProcessScheduleHits(float dt)
        {
            ProcessScheduleTileStackHits(dt);
            ProcessScheduleCellStackHits(dt);
            ProcessScheduleAttachmentHits(dt);
        }


        // TODO: Try merge this with ProcessScheduleTileStackHits
        private void ProcessScheduleCellStackHits(float dt)
        {
            activatedHitDelays.Clear();

            for (int i = 0; i < scheduledCellStackHits.Count; ++i)
            {
                var delayedHitData = scheduledCellStackHits[i];

                var currentDelay = delayedHitData.delay - dt;

                if (currentDelay <= 0)
                {
                    delayedHitData.hitData.cellStack.GetComponent<LockState>().Release();
                    activatedHitDelays.Push(i);
                    ApplyHit(delayedHitData.hitData);
                }
                else
                    scheduledCellStackHits[i] = new DelayedCellStackHitData(currentDelay, delayedHitData.hitData);
            }

            while (activatedHitDelays.Count > 0)
                scheduledCellStackHits.RemoveAt(activatedHitDelays.Pop());
        }

        private void ProcessScheduleTileStackHits(float dt)
        {
            activatedHitDelays.Clear();

            for (int i = 0; i < scheduledTileStackHits.Count; ++i)
            {
                var hitData = scheduledTileStackHits[i];

                var currentDelay = hitData.delay - dt;

                if (currentDelay <= 0)
                {
                    //if (hitData.tileHitData.tileStack.GetComponent<LockState>().IsLocked())
                    //    UnityEngine.Debug.LogFormat("Locked by {0}", hitData.tileHitData.tileStack.GetComponent<LockState>().KeyType());
                    // WARNING: Unlocking the lock here can be very dangerous. 
                    // TODO: Findout why the unlocking is needed
                    hitData.tileHitData.tileStack.GetComponent<LockState>().Release();
                    activatedHitDelays.Push(i);
                    ApplyHit(hitData.tileHitData);
                }
                else
                    scheduledTileStackHits[i] = new DelayedTileStackHitData(currentDelay, hitData.tileHitData);
            }

            while (activatedHitDelays.Count > 0)
                scheduledTileStackHits.RemoveAt(activatedHitDelays.Pop());
        }

        private void ProcessScheduleAttachmentHits(float dt)
        {
            activatedHitDelays.Clear();

            for (int i = 0; i < scheduledAttachmentHits.Count; ++i)
            {
                var delayedHitData = scheduledAttachmentHits[i];

                var currentDelay = delayedHitData.delay - dt;

                if (currentDelay <= 0)
                {
                    activatedHitDelays.Push(i);
                    ApplyHit(delayedHitData.hitData);
                }
                else
                    scheduledAttachmentHits[i] = new DelayedCellAttachmentHitData(currentDelay, delayedHitData.hitData);
            }

            while (activatedHitDelays.Count > 0)
                scheduledAttachmentHits.RemoveAt(activatedHitDelays.Pop());
        }


        private void ProcessGeneratedHits()
        {
            foreach (var tileData in generatedHitsData.tileStacksHitData)
                ApplyHit(tileData);

            foreach (var delayedTileData in generatedHitsData.delayedTileStacksHitData)
                ScheduleHit(delayedTileData);


            foreach (var cellData in generatedHitsData.cellStacksHitData)
                ApplyHit(cellData);

            foreach (var delayedCellData in generatedHitsData.delayedCellStacksHitData)
                ScheduleHit(delayedCellData);


            foreach (var attachmentData in generatedHitsData.cellAttachmentsHitData)
                ApplyHit(attachmentData);

            foreach (var delayedAttachmentData in generatedHitsData.delayedcellAttachmentsHitData)
                ScheduleHit(delayedAttachmentData);
        }

        private void ScheduleHit(DelayedTileStackHitData tileData)
        {
            var target = tileData.tileHitData.tileStack;
            target.GetComponent<LockState>().LockBy<HitApplyingKeyType>();

            scheduledTileStackHits.Add(tileData);
        }

        private void ScheduleHit(DelayedCellStackHitData cellData)
        {
            var target = cellData.hitData.cellStack;
            target.GetComponent<LockState>().LockBy<HitApplyingKeyType>();
            scheduledCellStackHits.Add(cellData);
        }

        private void ScheduleHit(DelayedCellAttachmentHitData delayedAttachmentData)
        {
            scheduledAttachmentHits.Add(delayedAttachmentData);
        }

        // TODO: Refactor this.
        void ApplyHit(TileStackHitData tileData)
        {
            var tileStack = tileData.tileStack;

            if (tileStack.IsDepleted())
                return;

            var topTile = tileStack.Top();

            if(topTile.DoesAcceptHit(tileData.hitType, tileData.hitCause))
            {
                topTile.Hit(tileData.hitType, tileData.hitCause);

                appliedHitsData.tilesStartedBeingHit.Add(topTile);


                if (topTile.IsDestroyed())
                    MarkAllRelatedTilesToDestroyAndPop(topTile);
                else
                    tileHittingPort.ExecuteEffectOn(topTile, () => appliedHitsData.tilesFinishedBeingHit.Add(topTile));
            }        
        }

        private void MarkAllRelatedTilesToDestroyAndPop(Tile topTile)
        {
            if (topTile is SlaveTile slaveTile)
                MarkToDestroyAndPopTilesOf(slaveTile.Master());
            else if (topTile is CompositeTile compositeTile)
                MarkToDestroyAndPopTilesOf(compositeTile);
            else
                MarkToDestroyAndPop(topTile);
        }

        private void MarkToDestroyAndPopTilesOf(CompositeTile compositeTile)
        {
            MarkToDestroyAndPop(compositeTile);

            foreach (var slave in compositeTile.Slaves())
                MarkToDestroyAndPop(slave);
        }
        
        private void MarkToDestroyAndPop(Tile topTile)
        {
            destructionData.tilesToDestroy.Add(topTile);
            topTile.Parent().Pop();
        }

        void ApplyHit(CellStackHitData hitData)
        {
            var cellStack = hitData.cellStack;

            if (cellStack.Top().IsDestroyed())
                return;

            var topCell = cellStack.Top();

            if (topCell.DoesAcceptHit(hitData.hitType))
            {

                topCell.Hit();

                appliedHitsData.cellsStartedBeingHit.Add(topCell);

                if (topCell.IsDestroyed())
                {
                    destructionData.cellsToDestroy.Add(topCell);
                    cellStack.Pop();
                }
                else
                    cellHittingPort.ExecuteEffectOn(cellStack, delegate { });
            }
        }

        private void ApplyHit(CellAttachmentHitData attachmentHitData)
        {
            var attachment = attachmentHitData.cellAttachment;
            if (attachment.IsDestroyed())
                return;

            attachment.Hit();

            if (attachment.IsDestroyed())
                destructionData.cellAttachmentsToDestroy.Add(attachment);
            else
                attachmentHittingPort.ExecuteEffectOn(attachment, delegate { });

        }

    }
}