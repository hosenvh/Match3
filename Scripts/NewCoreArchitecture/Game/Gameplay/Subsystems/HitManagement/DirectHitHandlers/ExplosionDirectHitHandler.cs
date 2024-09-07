namespace Match3.Game.Gameplay.HitManagement.DirectHitHandlers
{
    public class ExplosionDirectHitHandler : MechanicHitGenerator
    {

        ExplosionHitCause hitCause = new ExplosionHitCause();
        public ExplosionDirectHitHandler(HitGenerationSystem system) : base(system)
        {
        }

        public override void GenerateHits()
        {

            foreach (var explosionData in system.GetFrameData<ExplosionActivationData>().explosionsHitData)
            {
                system.BeginHitGroup();

                foreach (var hitData in explosionData.hitsData)
                {

                    system.TryGenerateDelayedCellStackHit(hitData.cellStack, hitCause, HitType.Direct, hitData.delay);

                    if (hitData.cellStack.HasTileStack())
                        system.TryGenerateDelayedTileStackHit(
                            hitData.cellStack.CurrentTileStack(),
                            hitCause,
                            hitData.delay,
                            HitType.Direct);
                }
                system.EndHitGroup();
            }
        }

    }
}