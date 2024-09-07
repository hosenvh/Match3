using Match3.Foundation.Base.EventManagement;
using Match3.Presentation.Gameplay;
using System;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;


namespace Match3.Game.DynamicDifficulty
{
    public class DifficultyDetector : EventListener
    {
        private enum DetectionState
        {
            NotDetected,
            Detected
        }

        private const string LAST_LEVEL_LOOSED_COUNT_KEY = "DynamicDifficulty_LastLevelLoosedCount";
        private const string DIFFICULTY_DETECTION_TIME_KEY = "DynamicDifficulty_DifficultyDetectionTime";
        private const string DIFFICULTY_DETECTED_KEY = "DynamicDifficulty_DifficultyDetected";

        private readonly UserProfileManager profile;
        private readonly int neededLooseCount;

        private int LooseCount
        {
            get => profile.LoadData(LAST_LEVEL_LOOSED_COUNT_KEY, 0);
            set => profile.SaveData(LAST_LEVEL_LOOSED_COUNT_KEY, value);
        }

        private DateTime DetectionTime
        {
            get => DateTime.FromFileTimeUtc(long.Parse(profile.LoadData(DIFFICULTY_DETECTION_TIME_KEY, "0")));
            set => profile.SaveData(DIFFICULTY_DETECTION_TIME_KEY, value == DateTime.MinValue ? "0" : value.ToFileTimeUtc().ToString());
        }

        private DetectionState State
        {
            get => profile.LoadData(DIFFICULTY_DETECTED_KEY, 0) == 0 ? DetectionState.NotDetected : DetectionState.Detected;
            set => profile.SaveData(DIFFICULTY_DETECTED_KEY, value == DetectionState.NotDetected ? 0 : 1);
        }

        public DifficultyDetector(UserProfileManager profile, EventManager eventManager, int neededLooseCount)
        {
            this.profile = profile;
            this.neededLooseCount = neededLooseCount;
            eventManager.Register(this);
        }

        public TimeSpan GetPassedTimeFromDetection()
        {
            return State == DetectionState.NotDetected ? TimeSpan.Zero : DateTime.UtcNow - DetectionTime;
        }

        public void Restart()
        {
            ChangeState(DetectionState.NotDetected);
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (State == DetectionState.NotDetected && (evt is LevelGaveUpEvent || evt is LevelAbortedEvent || evt is LevelRetriedEvent) && Base.gameManager.CurrentState is CampaignGameplayState)
            {
                LooseCount++;
                if (LooseCount >= neededLooseCount)
                    ChangeState(DetectionState.Detected);
            }
            else if (evt is LevelEndedEvent levelEndedEvent && levelEndedEvent.result == LevelResult.Win && Base.gameManager.CurrentState is CampaignGameplayState)
                ChangeState(DetectionState.NotDetected);
        }

        private void ChangeState(DetectionState state)
        {
            State = state;
            switch (state)
            {
                case DetectionState.NotDetected:
                    LooseCount = 0;
                    DetectionTime = DateTime.MinValue;
                    break;
                case DetectionState.Detected:
                    DetectionTime = DateTime.UtcNow;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}