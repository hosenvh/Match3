
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.HitManagement
{
    public partial class RopeHitGenerator : MechanicHitGenerator, HitListener
    {        
        HashSet<Rope> processedRopes = new HashSet<Rope>();

        RopeExplosionHitGenerator ropeExplosionHitGenerator;
        RopeNonExplosionHitGenerator ropeNonExplosionHitGenerator;
        RopeIterationHelper ropeIterationHelper;

        ExplosionActivationData explosionActivationData;

        public RopeHitGenerator(HitGenerationSystem system, CellStackBoard cellStackBoard) : base(system)
        {
            ropeExplosionHitGenerator = new RopeExplosionHitGenerator(this);
            ropeNonExplosionHitGenerator = new RopeNonExplosionHitGenerator(this);
            ropeIterationHelper = new RopeIterationHelper(cellStackBoard);
            explosionActivationData = system.GetFrameData<ExplosionActivationData>();
        }

        public override void GenerateHits()
        {
            ropeExplosionHitGenerator.GenerateHitsForExplosions(explosionActivationData, system);
        }

        public void OnCellStackHit(CellStackHitData hitData, HitCause hitCause)
        {
            ropeNonExplosionHitGenerator.GenerateNonExplosionHitFor(hitData, hitCause);
        }

        public void OnCellStackDelayedHit(DelayedCellStackHitData delayedHitData, HitCause hitCause)
        {
            ropeNonExplosionHitGenerator.GenerateDelayedNonExplosionHitFor(delayedHitData, hitCause);
        }

        public void OnHitGroupBegan()
        {
            ClearProcessedRopes();
        }

        public void OnHitGroupEnded()
        {

        }

        private void ClearProcessedRopes()
        {
            processedRopes.Clear();
        }

        private void TryGenerateHits(Rope rope)
        {
            if (processedRopes.Contains(rope) == false)
            {
                processedRopes.Add(rope);
                system.TryGenerateAttachmentHit(rope);
            }
        }

        private void TryGenerateHitsWithDelay(Rope rope, float delay)
        {
            if (processedRopes.Contains(rope) == false)
            {
                processedRopes.Add(rope);
                system.TryGenerateDelayedAttachmentHit(rope, delay);
            }
        }

        private void ForeachRopesArroundHitDo(Action<Rope> action, CellStack center)
        {
            ropeIterationHelper.ForeachRopesArroundHitDo(action, center);
        }
    }
}