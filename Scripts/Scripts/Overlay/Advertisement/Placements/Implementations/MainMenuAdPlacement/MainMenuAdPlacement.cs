using Match3.Game;
using Match3.Overlay.Advertisement.Placements.Implementations.TimeBasedAdPlacementBase;


namespace Match3.Overlay.Advertisement.Placements.Implementations.MainMenuAdPlacement
{
    public class MainMenuAdPlacement : TimeBasedAdPlacement
    {
        public const string SERVER_TIME_SAVE_KEY = "MainMenu_AdButton";

        public MainMenuAdPlacement(int availabilityGapDuration, Reward[] rewards, int availabilityLevel, int maxPlaysInDay) : base(availabilityGapDuration, rewards, availabilityLevel, maxPlaysInDay)
        {
        }

        protected override string GetServerTimeSaveKey()
        {
            return SERVER_TIME_SAVE_KEY;
        }

        public override string Name()
        {
            return "MainMenu_Ad";
        }
    }
}