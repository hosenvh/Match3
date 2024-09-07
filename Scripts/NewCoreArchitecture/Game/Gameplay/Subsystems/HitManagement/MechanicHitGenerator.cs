namespace Match3.Game.Gameplay.HitManagement
{
    public abstract class MechanicHitGenerator
    {
        protected HitGenerationSystem system;


        protected MechanicHitGenerator(HitGenerationSystem system)
        {
            this.system = system;

        }

        public abstract void GenerateHits();
    }
}