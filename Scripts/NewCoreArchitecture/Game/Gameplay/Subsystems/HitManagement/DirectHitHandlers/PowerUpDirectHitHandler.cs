namespace Match3.Game.Gameplay.HitManagement
{

    public class PowerUpDirectHitHandler : MechanicHitGenerator
    {
        PowerUpHitCause hitCause = new PowerUpHitCause();

        public PowerUpDirectHitHandler(HitGenerationSystem system) : base(system)
        {
        }

        public override void GenerateHits()
        {
            system.BeginHitGroup();

            foreach (var cellStack in system.GetFrameData<PowerUpActivationData>().affectedCellStacks)
            {
                system.TryGenerateCellStackHit(cellStack, hitCause, HitType.Direct);

                if(cellStack.HasTileStack())
                    system.TryGenerateTileStackHit(
                        cellStack.CurrentTileStack(),
                        hitCause,
                        HitType.Direct);
            }

            system.EndHitGroup();
        }
    }
}