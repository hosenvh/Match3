
using Match3.Game.Gameplay.CellAttachments;

namespace Match3.Game.Gameplay.HitManagement
{
    public partial class RopeHitGenerator
    {
        private class RopeNonExplosionHitGenerator
        {

            RopeHitGenerator mainRopeHitGenerator;

            public RopeNonExplosionHitGenerator(RopeHitGenerator mainRopeHitGenerator)
            {
                this.mainRopeHitGenerator = mainRopeHitGenerator;
            }

            public void GenerateNonExplosionHitFor(CellStackHitData hitData, HitCause hitCause)
            {
                if (ShouldProcessCellStackHit(hitData, hitCause))
                    mainRopeHitGenerator.ForeachRopesArroundHitDo(
                        TryGenerateHits, 
                        hitData.cellStack);
            }

            public void GenerateDelayedNonExplosionHitFor(DelayedCellStackHitData delayedHitData, HitCause hitCause)
            {
                if (ShouldProcessCellStackHit(delayedHitData.hitData, hitCause))
                    mainRopeHitGenerator.ForeachRopesArroundHitDo(
                        (rope) => TryGenerateHitsWithDelay(rope, delayedHitData.delay),
                        delayedHitData.hitData.cellStack);
            }

            private bool ShouldProcessCellStackHit(CellStackHitData hitData, HitCause hitCause)
            {
                return hitData.hitType == HitType.Direct && hitCause is ExplosionHitCause == false;
            }


            void TryGenerateHits(Rope rope)
            {
                mainRopeHitGenerator.TryGenerateHits(rope);
            }


            private void TryGenerateHitsWithDelay(Rope rope, float delay)
            {
                mainRopeHitGenerator.TryGenerateHitsWithDelay(rope, delay);
            }


        }
    }
}