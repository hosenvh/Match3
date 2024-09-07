

using Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic;

namespace Match3.Game.Gameplay.HitManagement.DirectHitHandlers
{

    public class RainbowDirectHitHandler : MechanicHitGenerator
    {
        public RainbowDirectHitHandler(HitGenerationSystem system) : base(system)
        {
        }

        public override void GenerateHits()
        {
            var activationData = system.GetFrameData<RainbowActivationData>();
            var hitCause = new RainbowHitCause(RainbowColorExtractor.ExtractFrom(activationData));

            system.BeginHitGroup();

            foreach (var tileStack in activationData.tileStackHits)
            {
                system.TryGenerateCellStackHit(tileStack.Parent(), hitCause, HitType.Direct);
                system.TryGenerateTileStackHit(tileStack, hitCause, HitType.Direct);
            }
            foreach (var cellStack in activationData.activatedRainbowCellStacks)
            {
                system.TryGenerateCellStackHit(cellStack, hitCause, HitType.Direct);
            }

            foreach (var hitData in activationData.delayedCellStackHits)
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