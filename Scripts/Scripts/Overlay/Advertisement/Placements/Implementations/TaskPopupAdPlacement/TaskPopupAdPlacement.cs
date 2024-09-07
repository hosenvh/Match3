using Match3.Game;
using Match3.Overlay.Advertisement.Placements.Implementations.TimeBasedAdPlacementBase;


namespace Match3.Overlay.Advertisement.Placements.Implementations.TaskPopupAdPlacement
{
    public class TaskPopupAdPlacement : TimeBasedAdPlacement
    {
        public const string SERVER_TIME_SAVE_KEY = "TaskMenu_AdButton";

        public TaskPopupAdPlacement(int availabilityGapDuration, Reward[] rewards, int availabilityLevel, int maxPlaysInDay) : base(availabilityGapDuration, rewards, availabilityLevel, maxPlaysInDay)
        {
        }

        public override string Name()
        {
            return "TaskMenu_Ad";
        }

        protected override string GetServerTimeSaveKey()
        {
            return SERVER_TIME_SAVE_KEY;
        }
    }
}