namespace Match3.Game.Gameplay.SubSystems.WinSequence
{
    public abstract class State
    {

        protected GameplayController gpc;
        protected LevelWinSequenceSystem system;

        public void Setup(LevelWinSequenceSystem winSequenceSystem, GameplayController gameplayController)
        {
            this.gpc = gameplayController;
            this.system = winSequenceSystem;
            InternalSetup();

        }

        protected abstract void InternalSetup();

        public abstract void Update(float dt);


        
    }
}