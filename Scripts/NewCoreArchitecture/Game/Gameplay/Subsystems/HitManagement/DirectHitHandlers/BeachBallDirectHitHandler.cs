
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.ExplosionManagement;
using Match3.Game.Gameplay.SubSystemsData.FrameData.BeachBallMechanic;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.HitManagement.DirectHitHandlers
{
    public class BeachBallDirectHitHandler : MechanicHitGenerator
    {
        HitCause hitCause = new BeachBallHitCause();

        ExplosionActivationGatheringSystem explosionActivationGatheringSystem;

        public BeachBallDirectHitHandler(HitGenerationSystem system, GameplayController gameplayController) : base(system)
        {
            this.explosionActivationGatheringSystem = gameplayController.GetSystem< ExplosionActivationGatheringSystem>();
        }

        public override void GenerateHits()
        {
            var activationData = system.GetFrameData<BeachBallActivationData>();

            system.BeginHitGroup();
            foreach (var hitData in activationData.delayedCellStackHits)
            {
                system.TryGenerateDelayedCellStackHit(hitData.cellStack, hitCause, HitType.Direct, hitData.delay);

                if (hitData.cellStack.HasTileStack())
                    HitTileStack(hitData.cellStack.CurrentTileStack(), hitData.delay);     
            }

            system.EndHitGroup();
        }

        void HitTileStack(TileStack tileStack, float delay)
        {
            system.TryGenerateDelayedTileStackHit(
                    tileStack,
                    hitCause,
                    delay, 
                    HitType.Direct);
        }
    }
}