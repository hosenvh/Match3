using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Presentation.Gameplay;


namespace Match3.Overlay.Analytics.LevelEntries
{
    public abstract class LevelEntriesAnalyticsHandler : GameplayAnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case LevelStartResourceConsumingEvent _:
                    HandleLevelStart(GetLoadedLevelNumber());
                    break;
                case LevelStartedWithBoosters levelStartedWithBoosters:
                    if (levelStartedWithBoosters.HasDoubleBomb)
                        HandleLevelStartWithDoubleBomb(GetLoadedLevelNumber());
                    if (levelStartedWithBoosters.HasRainbow)
                        HandleLevelStartWithRainbow(GetLoadedLevelNumber());
                    if (levelStartedWithBoosters.HasTntRainbow)
                        HandleLevelStartWithTNTRainbow(GetLoadedLevelNumber());
                    break;
                case LevelAbortedEvent _:
                    HandleLevelAbort(GetLoadedLevelNumber());
                    break;
                case LevelScoredEvent data:
                    HandleLevelWin(data.score, GetLoadedLevelNumber());
                    break;
                case LevelEndedEvent levelEndedEvent:
                    HandleLevelEnded(GetLoadedLevelNumber());
                    if (levelEndedEvent.result == LevelResult.Lose)
                        HandleLevelLose(GetLoadedLevelNumber());
                    break;
                case LevelGaveUpEvent _:
                    HandleLevelGaveUp(GetLoadedLevelNumber());
                    break;
                case LevelRetriedEvent _:
                    HandleLevelRetry(GetLoadedLevelNumber());
                    break;
                case LevelResumeWithExtraMoveEvent data:
                    HandleContinueWithExtraMove(data.cost, GetLoadedLevelNumber());
                    break;
            }
        }

        private int GetLoadedLevelNumber()
        {
            return ((GameplayState) Base.gameManager.CurrentState).CurrentLevelIndex() + 1;
        }


        protected abstract void HandleLevelStart(int levelNumber);
        protected abstract void HandleLevelStartWithDoubleBomb(int levelNumber);
        protected abstract void HandleLevelStartWithRainbow(int levelNumber);
        protected abstract void HandleLevelStartWithTNTRainbow(int levelNumber);
        protected abstract void HandleLevelAbort(int levelNumber);
        protected abstract void HandleLevelWin(int score, int levelNumber);
        protected abstract void HandleLevelEnded(int levelNumber);
        protected abstract void HandleLevelLose(int levelNumber);
        protected abstract void HandleLevelGaveUp(int levelNumber);
        protected abstract void HandleLevelRetry(int levelNumber);
        protected abstract void HandleContinueWithExtraMove(int boughtExtraMoveCost, int levelNumber);
    }
}