using Match3.Game.Gameplay.ExplosionManagement;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RocketMechanic;
using Match3.Game.Gameplay.Tiles;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.HitManagement.DirectHitHandlers
{

    public class RocketBoxDirectHitHandler : MechanicHitGenerator
    {

        struct RocketBoxHitCause : HitCause
        { }

        ExplosionActivationGatheringSystem explosionActivationGatheringSystem;

        RocketBoxHitCause rocketBoxHitCause = new RocketBoxHitCause();

        public RocketBoxDirectHitHandler(HitGenerationSystem system, GameplayController gameplayController) : base(system)
        {
            this.explosionActivationGatheringSystem = gameplayController.GetSystem<ExplosionActivationGatheringSystem>();
        }

        // TODO: Refactor this.
        public override void GenerateHits()
        {
            system.BeginHitGroup();

            foreach (var cellstack in system.GetFrameData<RocketHitData>().cellStacks)
            {
                if (cellstack.HasTileStack()
                    && cellstack.CurrentTileStack().IsDepleted() == false
                    && cellstack.CurrentTileStack().GetComponent<LockState>().IsFree())
                {

                    if (HasTileOnTop<ExplosiveTile>(cellstack))
                    {
                        explosionActivationGatheringSystem.Enqueue(cellstack.CurrentTileStack());
                    }
                    else
                    {
                        system.TryGenerateCellStackHit(cellstack, rocketBoxHitCause, HitType.Direct);
                        system.TryGenerateTileStackHit(cellstack.CurrentTileStack(), rocketBoxHitCause, HitType.Direct);
                    }
                }
                else if (cellstack.HasTileStack() == false && cellstack.GetComponent<LockState>().IsFree())
                    system.TryGenerateCellStackHit(cellstack, rocketBoxHitCause, HitType.Direct);
            }

            system.EndHitGroup();
        }


    }
}