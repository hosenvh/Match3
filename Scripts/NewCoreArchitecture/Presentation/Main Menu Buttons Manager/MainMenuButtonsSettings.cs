using Match3.Presentation.MainMenuLayout;


namespace Match3.Presentation.MainMenu
{
    public struct MainMenuButtonSetting
    {
        public int Priority { get; }
        public AlignmentSide AlignmentSide { get; }

        public MainMenuButtonSetting(int priority, AlignmentSide alignmentSide)
        {
            Priority = priority;
            AlignmentSide = alignmentSide;
        }
    }

    public static class MainMenuButtonsSettings
    {
        public static MainMenuButtonSetting LuckySpinner => new MainMenuButtonSetting(priority: 0 , alignmentSide: AlignmentSide.Left);
        public static MainMenuButtonSetting Update => new MainMenuButtonSetting(priority: 1 , alignmentSide: AlignmentSide.Left);
        public static MainMenuButtonSetting EnabledNeighbourhoodChallenge => new MainMenuButtonSetting(priority: 2 , alignmentSide: AlignmentSide.Left);
        public static MainMenuButtonSetting DisabledNeighbourhoodChallenge => new MainMenuButtonSetting(priority: 2 , alignmentSide: AlignmentSide.Left);
        public static MainMenuButtonSetting NeighborhoodIntro => new MainMenuButtonSetting(priority: 3, alignmentSide: AlignmentSide.Left);
        public static MainMenuButtonSetting Ads => new MainMenuButtonSetting(priority: 4 , alignmentSide: AlignmentSide.Left);

        public static MainMenuButtonSetting LiveopsSpecialOffers => new MainMenuButtonSetting(priority: 0 , alignmentSide: AlignmentSide.Right);
        public static MainMenuButtonSetting BookReading => new MainMenuButtonSetting(priority: 1 , alignmentSide: AlignmentSide.Right);
        public static MainMenuButtonSetting ReferralCenter => new MainMenuButtonSetting(priority: 2 , alignmentSide: AlignmentSide.Right);
        public static MainMenuButtonSetting SpecialOffer => new MainMenuButtonSetting(priority: 3 , alignmentSide: AlignmentSide.Right);
        public static MainMenuButtonSetting PiggyBank => new MainMenuButtonSetting(priority: 4 , alignmentSide: AlignmentSide.Right);
        public static MainMenuButtonSetting SeasonPass => new MainMenuButtonSetting(priority: 5 , alignmentSide: AlignmentSide.Right);
        public static MainMenuButtonSetting SeasonPassSpecialOffers => new MainMenuButtonSetting(priority: 6, alignmentSide: AlignmentSide.Right);
        public static MainMenuButtonSetting Joker => new MainMenuButtonSetting(priority: 7 , alignmentSide: AlignmentSide.Right);
        public static MainMenuButtonSetting HerbariumCandy => new MainMenuButtonSetting(priority: 8 , alignmentSide: AlignmentSide.Right);
        public static MainMenuButtonSetting DogTraining => new MainMenuButtonSetting(priority: 9 , alignmentSide: AlignmentSide.Right);
    }
}