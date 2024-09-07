namespace Match3.Game.Gameplay.HitManagement.DirectHitHandlers
{
    public class MatchDirectHitHandler : MechanicHitGenerator
    {

        CreatedMatchesData createdMatchesData;

        MatchHitCause hitCause = new MatchHitCause();

        public MatchDirectHitHandler(HitGenerationSystem system) : base(system)
        {
            createdMatchesData = system.GetFrameData<CreatedMatchesData>();
        }

        public override void GenerateHits()
        {
            foreach (var match in createdMatchesData.data)
            {
                system.BeginHitGroup();
                foreach (var tileStack in match.tileStacks)
                {
                    system.TryGenerateCellStackHit(tileStack.Parent(), hitCause, HitType.Direct);
                    system.TryGenerateTileStackHit(
                        tileStack,
                        hitCause,
                        HitType.Direct);
                }
                system.EndHitGroup();
            }
        }
    }
}