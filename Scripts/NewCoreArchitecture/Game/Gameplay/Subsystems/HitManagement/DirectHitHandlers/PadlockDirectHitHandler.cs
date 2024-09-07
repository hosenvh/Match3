using Match3.Game.Gameplay.SubSystems.PadlockMechanic;
using Match3.Game.Gameplay.SubSystems.VacuumCleanerMechanic;

namespace Match3.Game.Gameplay.HitManagement.DirectHitHandlers
{
    public class PadlockDirectHitHandler : MechanicHitGenerator
    {
        struct PadlockHitCause : HitCause
        { }


        PadlockHitCause cause = new PadlockHitCause();

        public PadlockDirectHitHandler(HitGenerationSystem system) : base(system)
        {
        }

        public override void GenerateHits()
        {
            system.BeginHitGroup();
            foreach (var padlock in system.GetFrameData<PadlockHitData>().padlocks)
            {
                system.TryGenerateTileStackHit(
                            padlock.Parent(),
                            cause,
                            HitType.Direct);

            }
            system.EndHitGroup();
        }
    }
}