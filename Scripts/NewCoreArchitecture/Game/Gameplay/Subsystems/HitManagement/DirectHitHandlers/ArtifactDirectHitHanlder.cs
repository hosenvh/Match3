using Match3.Game.Gameplay.SubSystemsData.FrameData.ArtifactMechanic;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.HitManagement.DirectHitHandlers
{
    public class ArtifactDirectHitHanlder : MechanicHitGenerator
    {
        struct ArtifactHitCause : HitCause
        { }

        public ArtifactDirectHitHanlder(HitGenerationSystem system) : base(system)
        {
        }

        public override void GenerateHits()
        {
            system.BeginHitGroup();

            foreach (var cellStack in system.GetFrameData<ArtifactHitData>().cellStackToHitIfColored)
            {
                if (cellStack.HasTileStack()
                    && cellStack.CurrentTileStack().IsDepleted() == false
                    && cellStack.CurrentTileStack().Top() is ColoredBead
                    && cellStack.CurrentTileStack().GetComponent<LockState>().IsFree())
                {
                    system.TryGenerateTileStackHit(
                        cellStack.CurrentTileStack(),
                        new ArtifactHitCause(),
                        HitType.Direct);
                }
            }

            system.EndHitGroup();
        }
    }
}