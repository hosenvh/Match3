using Match3.Game.Gameplay.SubSystemsData.FrameData.BeachBallMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;

namespace Match3.Game.Gameplay.HitManagement.DirectHitHandlers
{
    public class GeneralDirectHitHandler : MechanicHitGenerator
    {
        HitCause hitCause = new GeneralHitCause();
        GeneralHitData generalHitData;
        
        public GeneralDirectHitHandler(HitGenerationSystem system) : base(system)
        {
            this.generalHitData = system.GetFrameData<GeneralHitData>();
        }

        public override void GenerateHits()
        {
            var activationData = system.GetFrameData<BeachBallActivationData>();

            system.BeginHitGroup();
            foreach (var cellStack in generalHitData.cellStacksToHit)
            {
                system.TryGenerateCellStackHit(cellStack, hitCause, HitType.Direct);

                if (cellStack.HasTileStack())
                    system.TryGenerateTileStackHit(cellStack.CurrentTileStack(), hitCause, HitType.Direct);
            }

            system.EndHitGroup();
        }
    }
}