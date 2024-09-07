using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.ExplosionManagement;
using Match3.Game.Gameplay.HitManagement.DirectHitHandlers;
using Match3.Game.Gameplay.HitManagement.SideHitHandlers;
using System.Collections.Generic;
using Match3.Utility.GolmoradLogging;
using UnityEngine;

namespace Match3.Game.Gameplay.HitManagement
{

    public interface HitSystemKeyType : KeyType {}
    public interface HitCause{ }
    public enum HitType { Direct, Side };

    public interface HitListener
    {
        void OnHitGroupBegan();
        void OnHitGroupEnded();

        void OnCellStackHit(CellStackHitData hitData, HitCause hitCause);
        void OnCellStackDelayedHit(DelayedCellStackHitData delayedHitData, HitCause hitCause);
    }

    // TODO: The process of generating hits for tiles and cells is confusing. Refactor this shit.
    // TODO: Really refactor this shit.
    [AfterAttribute(typeof(Matching.MatchDetectionSystem))]
    public class HitGenerationSystem : GameplaySystem
    {
        private List<MechanicHitGenerator> directHitHandlers = new List<MechanicHitGenerator>();
        private List<MechanicHitGenerator> sideHitHandlers = new List<MechanicHitGenerator>();


        GeneratedHitsData generatedHitsData;

        HitGroup hitGroup = new HitGroup();

        // NOTE: This can be a list if necessary.
        HitListener hitListener;

        public HitGenerationSystem(GameplayController gameplayController) : base(gameplayController)
        {

            directHitHandlers.Add(new MatchDirectHitHandler(this));
            directHitHandlers.Add(new ExplosionDirectHitHandler(this));
            directHitHandlers.Add(new RainbowDirectHitHandler(this));
            directHitHandlers.Add(new PowerUpDirectHitHandler(this));
            directHitHandlers.Add(new ArtifactDirectHitHanlder(this));
            directHitHandlers.Add(new RocketBoxDirectHitHandler(this, gameplayController));
            directHitHandlers.Add(new VacuumCleanerDirectHitHandler(this, gameplayController));
            directHitHandlers.Add(new PadlockDirectHitHandler(this));
            directHitHandlers.Add(new BeachBallDirectHitHandler(this, gameplayController));
            directHitHandlers.Add(new GeneralDirectHitHandler(this));

            sideHitHandlers.Add(new MatchSideHitHandler(this));
            sideHitHandlers.Add(new RainbowSideHitHandler(this));


            var ropeHitGenerator = new RopeHitGenerator(this, gameplayController.GameBoard().CellStackBoard());
            hitListener = ropeHitGenerator;
            directHitHandlers.Add(ropeHitGenerator);

            this.generatedHitsData = GetFrameData<GeneratedHitsData>();
        }

        public override void Update(float dt)
        {
            foreach (var handlder in directHitHandlers)
                handlder.GenerateHits();

            foreach (var handlder in sideHitHandlers)
                handlder.GenerateHits();
        }

        public void BeginHitGroup()
        {
            hitGroup.Clear();
            hitListener.OnHitGroupBegan();
        }

        public void EndHitGroup()
        {
            hitGroup.Clear();
            hitListener.OnHitGroupEnded();
        }

        public void TryGenerateTileStackHit(TileStack tileStack, HitCause hitCause, HitType hitType)
        {
            if (IsTileStackValidForHitting(tileStack, hitType) == false || hitGroup.IsNewEntity(tileStack) == false)
                return;

            hitGroup.AddEntityOf(tileStack);
            generatedHitsData.tileStacksHitData.Add(new TileStackHitData(tileStack, hitCause, hitType));
        }

        public void TryGenerateDelayedTileStackHit(TileStack tileStack, HitCause hitCause, float delay, HitType hitType)
        {
            if (IsTileStackValidForHitting(tileStack, hitType) == false || hitGroup.IsNewEntity(tileStack) == false)
                return;

            hitGroup.AddEntityOf(tileStack);

            var data = new DelayedTileStackHitData(delay, new TileStackHitData(tileStack, hitCause, HitType.Direct));
            generatedHitsData.delayedTileStacksHitData.Add(data);
        }

        public void TryGenerateDelayedCellStackHit(CellStack cellStack, HitCause hitCause, HitType hitType, float delay)
        {
            if (IsCellStackValidForHitting(cellStack) == false)
                return;

            var data = new DelayedCellStackHitData(delay, new CellStackHitData(cellStack, hitType));
            generatedHitsData.delayedCellStacksHitData.Add(data);
            hitListener.OnCellStackDelayedHit(data, hitCause);
        }

        public void TryGenerateCellStackHit(CellStack cellStack, HitCause hitCause, HitType hitType)
        {
            if (IsCellStackValidForHitting(cellStack) == false)
                return;

            var hitData = new CellStackHitData(cellStack, hitType);
            generatedHitsData.cellStacksHitData.Add(hitData);
            hitListener.OnCellStackHit(hitData, hitCause);
        }

        public bool IsCellStackValidForHitting(CellStack cellStack)
        {
            // TODO: This is a sanity check for now. Find the cause and remove this later.
            if(cellStack == null)
            {
                DebugPro.LogError<CoreGameplayLogTag>("CellStack for hitting is null");
                return false;
            }

            var currentTileStack = cellStack.CurrentTileStack();

            return (currentTileStack == null || currentTileStack.IsDepleted())
                || (HasOneTile(currentTileStack) && PropagatesHitToCell(currentTileStack.Top()));
        }

        public void TryGenerateAttachmentHit(HitableCellAttachment cellAttachment)
        {
            if (IsAttachmentValidForHitting(cellAttachment) == false)
                return;

            var hitData = new CellAttachmentHitData(cellAttachment);
            generatedHitsData.cellAttachmentsHitData.Add(hitData);
        }

        public void TryGenerateDelayedAttachmentHit(HitableCellAttachment cellAttachment, float delay)
        {
            if (IsAttachmentValidForHitting(cellAttachment) == false)
                return;

            var hitData = new DelayedCellAttachmentHitData(delay, new CellAttachmentHitData(cellAttachment));
            generatedHitsData.delayedcellAttachmentsHitData.Add(hitData);
        }

        private bool IsAttachmentValidForHitting(HitableCellAttachment cellAttachment)
        {
            return cellAttachment.IsDestroyed() == false;
        }

        // WARNING: TRY NOT TO USE THIS.
        public void ForceGenerateCellStackHitFor(CellStack cellStack)
        {
            generatedHitsData.cellStacksHitData.Add(new CellStackHitData(cellStack, HitType.Direct));
        }

        bool IsTileStackValidForHitting(TileStack tileStack, HitType hitType)
        {
            return tileStack.IsDestroyed() == false && tileStack.IsDepleted() == false
                && DoesTileAcceptHit(tileStack.Top(), hitType) == true;
        }

        bool DoesTileAcceptHit(Tile tile, HitType hitType)
        {
            switch(hitType)
            {
                case HitType.Direct:
                    return tile.componentCache.tileHitProperties.AcceptsDirectHit();
                case HitType.Side:
                    return tile.componentCache.tileHitProperties.AcceptsSideHit();
            }

            return false;
        }

        public GameBoard GameBoard()
        {
            return gameplayController.GameBoard();
        }

        bool PropagatesHitToCell(Tile tile)
        {
            return tile.componentCache.tileHitProperties.PropagatesHitToCell();
        }

        bool HasOneTile(TileStack tileStack)
        {
            return tileStack.Stack().Count == 1;
        }
    }
}