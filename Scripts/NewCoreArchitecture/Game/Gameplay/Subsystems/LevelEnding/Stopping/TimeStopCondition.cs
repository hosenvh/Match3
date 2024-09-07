namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{
    public class TimeStopCondition : StopConditinon
    {
        float remainingTime;

        public TimeStopCondition(float totalTime)
        {
            remainingTime = totalTime;
        }

        public float RemainingTime()
        {
            return remainingTime;
        }

        public bool IsSatisfied()
        {
            return remainingTime <= 0;
        }

        public void Update(float dt, LevelStoppingSystem system)
        {
            remainingTime -= dt;
        }
    }
}