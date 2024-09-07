using Match3;
using Match3.Foundation.Base.ServiceLocating;


namespace I2.Loc
{
    public static class ScriptLocalization
    {

        public static class Message
        {
            public static LocalizedStringTerm ExitGame => new LocalizedStringTerm("Message/ExitGame");
            public static LocalizedStringTerm NewVersionAvailable => new LocalizedStringTerm("Message/NewVersionAvailable");
            public static LocalizedStringTerm FullTicket => new LocalizedStringTerm("Message/FullTicket");
            public static LocalizedStringTerm NotProperName => new LocalizedStringTerm("Message/NotProperName");
            public static LocalizedStringTerm YouGotNotificationGift => new LocalizedStringTerm("Message/YouGotNotificationGift");
        }

        public static class Message_AccountTab
        {
            public static LocalizedStringTerm CloudSaveDeactivated => new LocalizedStringTerm("Message/AccountTab/CloudSaveDeactivated");
            public static LocalizedStringTerm GameIsSaved => new LocalizedStringTerm("Message/AccountTab/GameIsSaved");
            public static LocalizedStringTerm GameNotSaved => new LocalizedStringTerm("Message/AccountTab/GameNotSaved");
            public static LocalizedStringTerm LoadProgressFailed => new LocalizedStringTerm("Message/AccountTab/LoadProgressFailed");
            public static LocalizedStringTerm RestoreAccountConfirmation => new LocalizedStringTerm("Message/AccountTab/RestoreAccountConfirmation");
            public static LocalizedStringTerm SaveProgressFailed => new LocalizedStringTerm("Message/AccountTab/SaveProgressFailed");
            public static LocalizedStringTerm SaveProgressSuccessfull => new LocalizedStringTerm("Message/AccountTab/SaveProgressSuccessfull");
            public static LocalizedStringTerm LoadProgressSuccessfull => new LocalizedStringTerm("Message/AccountTab/LoadProgressSuccessfull");
            public static LocalizedStringTerm UserHasNotSaved => new LocalizedStringTerm("Message/AccountTab/UserHasNotSaved");
        }

        public static class Message_Advertisement
        {
            public static LocalizedStringTerm AskToWatchVideo => new LocalizedStringTerm("Message/Advertisement/AskToWatchVideo");
            public static LocalizedStringTerm WatchForExtraMoves => new LocalizedStringTerm("Message/Advertisement/WatchForExtraMoves");
            public static LocalizedStringTerm NoAvailableVideo => new LocalizedStringTerm("Message/Advertisement/NoAvailableVideo");
            public static LocalizedStringTerm Timeout => new LocalizedStringTerm("Message/Advertisement/Timeout");
            public static LocalizedStringTerm WatchVideoCompletely => new LocalizedStringTerm("Message/Advertisement/WatchVideoCompletely");
            public static LocalizedStringTerm RewardGetDouble => new LocalizedStringTerm("Message/Advertisement/RewardGetDouble");
            public static LocalizedStringTerm WatchVideoForDoubleReward => new LocalizedStringTerm("Message/Advertisement/WatchVideoForDoubleReward");
            public static LocalizedStringTerm YouGotExtraMove => new LocalizedStringTerm("Message/Advertisement/YouGotExtraMove");
        }

        public static class Message_Campaign
        {
            public static LocalizedStringTerm YouFinishedAllLevels => new LocalizedStringTerm("Message/Campaign/YouFinishedAllLevels");
            public static LocalizedStringTerm YouFinishedAllLevelsWithNcOffer => new LocalizedStringTerm("Message/Campaign/YouFinishedAllLevelsWithNcOffer");
            public static LocalizedStringTerm ExitGameplay => new LocalizedStringTerm("Message/Campaign/ExitGameplay");
            public static LocalizedStringTerm ExitGameplayWhileHavingInfiniteLife => new LocalizedStringTerm("Message/Campaign/ExitGameplayWhileHavingInfiniteLife");
        }

        public static class Message_CloudSave
        {
            public static LocalizedStringTerm InstallPlayGamesToSave => new LocalizedStringTerm("Message/CloudSave/InstallPlayGamesToSave");
            public static LocalizedStringTerm CantLoginToCloud => new LocalizedStringTerm("Message/CloudSaveStatusDesc/MedrickAccount/AuthFailed");
        }

        public static class Message_GameOver
        {
            public static LocalizedStringTerm OutOfGas => new LocalizedStringTerm("Message/GameOver/OutOfGas");
            public static LocalizedStringTerm OutOfMove => new LocalizedStringTerm("Message/GameOver/OutOfMove");
        }

        public static class Message_GamePlay
        {
            public static LocalizedStringTerm BoosterUnlockLevel => new LocalizedStringTerm("Message/GamePlay/BoosterUnlockLevel");
            public static LocalizedStringTerm PowerUpUnlockLevel => new LocalizedStringTerm("Message/Gameplay/PowerUpUnlockLevel");
            public static LocalizedStringTerm Resume => new LocalizedStringTerm("Message/GamePlay/Resume");
        }

        public static class Message_General
        {
            public static LocalizedStringTerm Wait => new LocalizedStringTerm("Message/General/Wait");
            public static LocalizedStringTerm YouGotItBefore => new LocalizedStringTerm("Message/General/YouGotItBefore");
        }

        public static class Message_GiftCode
        {
            public static LocalizedStringTerm FirstEnterYourCode => new LocalizedStringTerm("Message/GiftCode/FirstEnterYourCode");
            public static LocalizedStringTerm ExpiredCode => new LocalizedStringTerm("Message/GiftCode/ExpiredCode");
            public static LocalizedStringTerm FullCapacity => new LocalizedStringTerm("Message/GiftCode/FullCapacity");
            public static LocalizedStringTerm UsedCode => new LocalizedStringTerm("Message/GiftCode/UsedCode");
            public static LocalizedStringTerm WrongCode => new LocalizedStringTerm("Message/GiftCode/WrongCode");
        }

        public static class Message_JoySystem
        {
            public static LocalizedStringTerm ThanksForRating => new LocalizedStringTerm("Message/JoySystem/ThanksForRating");
        }

        public static class Message_LifePopup
        {
            public static LocalizedStringTerm LifesIsFull => new LocalizedStringTerm("Message/LifePopup/LifesIsFull");
            public static LocalizedStringTerm YouGotThreeLives => new LocalizedStringTerm("Message/LifePopup/YouGotThreeLives");
        }

        public static class Message_TicketPopup
        {
            public static LocalizedStringTerm YouGotThreeTickets => new LocalizedStringTerm("Message/TicketPopup/YouGotThreeTickets");
        }

        public static class Message_LuckySpinner
        {
            public static LocalizedStringTerm LuckSpinnerAdsBasedIntro => new LocalizedStringTerm("Message/LuckySpinner/LuckSpinnerAdsBasedIntro");
            public static LocalizedStringTerm LuckySpinnerRequireInternet => new LocalizedStringTerm("Message/LuckySpinner/SpinnerRequirInternet");
        }

        public static class Message_MainMenuAd
        {
            public static LocalizedStringTerm MinutesLeftToAvailability => new LocalizedStringTerm("Message/MainMenuAd/MinutesLeftToAvailability");
            public static LocalizedStringTerm HoursLeftToAvailability => new LocalizedStringTerm("Message/MainMenuAd/HoursLeftToAvailability");
            public static LocalizedStringTerm AdPlacementNonTimeBasedConditionsAreNotSatisfied => new LocalizedStringTerm("Message/MainMenuAd/AdPlacementNonTimeBasedConditionsAreNotSatisfied");
        }

        public static class Message_NeighborhoodChallenge
        {
            public static LocalizedStringTerm YouAreBan => new LocalizedStringTerm("Message/NeighborhoodChallenge/YouAreBan");
            public static LocalizedStringTerm NcIsClose => new LocalizedStringTerm("Message/NeighborhoodChallenge/NcIsClose");
            public static LocalizedStringTerm ExitGamePlay => new LocalizedStringTerm("Message/NeighborhoodChallenge/ExitGamePlay");
            public static LocalizedStringTerm LevelChanged => new LocalizedStringTerm("Message/NeighborhoodChallenge/LevelChanged");
            public static LocalizedStringTerm ChallengeIsFinished => new LocalizedStringTerm("Message/NeighborhoodChallenge/ChallengeIsFinished");
        }

        public static class Message_Network
        {
            public static LocalizedStringTerm InternetIsNotConnect => new LocalizedStringTerm("Message/Network/InternetIsNotConnect");
            public static LocalizedStringTerm ServerNotResponse => new LocalizedStringTerm("Message/Network/ServerNotResponse");
            public static LocalizedStringTerm ServerError => new LocalizedStringTerm("Message/Network/ServerError");
            public static LocalizedStringTerm InternetConnectionFailedTryLater => new LocalizedStringTerm("Message/Network/InternetConnectionFailedTryLater");
            public static LocalizedStringTerm AskForInternet => new LocalizedStringTerm("Message/Network/AskForInternet");
        }

        public static class Message_Purchase
        {
            public static LocalizedStringTerm NotEnoughKey => new LocalizedStringTerm("Message/Purchase/NotEnoughKey");
            public static LocalizedStringTerm AskPurchaseBooster => new LocalizedStringTerm("Message/Purchase/AskPurchaseBooster");
            public static LocalizedStringTerm AskPurchasePowerUp => new LocalizedStringTerm("Message/Purchase/AskPurchasePowerUp");
            public static LocalizedStringTerm NotEnoughCoin => new LocalizedStringTerm("Message/Purchase/NotEnoughCoin");
            public static LocalizedStringTerm NeedBazaarToPurchase => new LocalizedStringTerm("Message/Purchase/NeedBazaarToPurchase");
            public static LocalizedStringTerm AskForPurchaseSkin => new LocalizedStringTerm("Message/Purchase/AskForPurchaseSkin");
        }

        public static class Message_ReferralMarketing
        {
            public static LocalizedStringTerm DontTryOwnReferralCode => new LocalizedStringTerm("Message/ReferralMarketing/DontTryOwnReferralCode");
            public static LocalizedStringTerm WrongReferralCode => new LocalizedStringTerm("Message/ReferralMarketing/WrongReferralCode");
            public static LocalizedStringTerm AlreadyUsedReferralCode => new LocalizedStringTerm("Message/ReferralMarketing/AlreadyUsedReferralCode");
            public static LocalizedStringTerm AskToEnterReferralCode => new LocalizedStringTerm("Message/ReferralMarketing/AskToEnterReferralCode");
            public static LocalizedStringTerm HasNewReferredPlayer => new LocalizedStringTerm("Message/ReferralMarketing/HasNewReferredPlayer");
        }

        public static class Message_Shop
        {
            public static LocalizedStringTerm InternetRequirment => new LocalizedStringTerm("Message/Shop/InternetRequirment");
            public static LocalizedStringTerm IncompletePurchase => new LocalizedStringTerm("Message/Shop/IncompletePurchase");
            public static LocalizedStringTerm PurchaseFailedWithMoneyReturnPromise => new LocalizedStringTerm("Message/Shop/PurchaseFailedWithMoneyReturnPromise");
            public static LocalizedStringTerm FailedPurchase => new LocalizedStringTerm("Message/Shop/FailedPurchase");
            public static LocalizedStringTerm CoinPackageCollected => new LocalizedStringTerm("Message/Shop/CoinPackageCollected");
        }

        public static class Message_Tasks
        {
            public static LocalizedStringTerm NoMoreTask => new LocalizedStringTerm("Message/Tasks/NoMoreTask");
            public static LocalizedStringTerm NotEnoughStar => new LocalizedStringTerm("Message/Tasks/NotEnoughStar");
            public static LocalizedStringTerm WatchVideo => new LocalizedStringTerm("Message/Tasks/WatchVideo");
        }

        public static class Message_HiddenReward
        {
            public static LocalizedStringTerm FoundHiddenReward => new LocalizedStringTerm("Message/HiddenReward/FoundHiddenReward");
        }

        public static class Misc
        {
            public static LocalizedStringTerm TimeSeparator => new LocalizedStringTerm("Misc/TimeSeparator");
            public static LocalizedStringTerm Day => new LocalizedStringTerm("Misc/Day");
            public static LocalizedStringTerm Hour => new LocalizedStringTerm("Misc/Hour");
            public static LocalizedStringTerm Minute => new LocalizedStringTerm("Misc/Minute");
            public static LocalizedStringTerm Second => new LocalizedStringTerm("Misc/Second");
        }

        public static class Misc_CharacterNames
        {
            public static LocalizedStringTerm CarDriver => new LocalizedStringTerm("Misc/CharacterNames/CarDriver");
            public static LocalizedStringTerm Parrot => new LocalizedStringTerm("Misc/CharacterNames/Parrot");
            public static LocalizedStringTerm Dad => new LocalizedStringTerm("Misc/CharacterNames/Dad");
            public static LocalizedStringTerm Ghalisho => new LocalizedStringTerm("Misc/CharacterNames/Ghalisho");
            public static LocalizedStringTerm Khatereh => new LocalizedStringTerm("Misc/CharacterNames/Khatereh");
            public static LocalizedStringTerm Mahboobeh => new LocalizedStringTerm("Misc/CharacterNames/Mahboobeh");
            public static LocalizedStringTerm Man => new LocalizedStringTerm("Misc/CharacterNames/Man");
            public static LocalizedStringTerm Masood => new LocalizedStringTerm("Misc/CharacterNames/Masood");
            public static LocalizedStringTerm Mother => new LocalizedStringTerm("Misc/CharacterNames/Mother");
            public static LocalizedStringTerm Pezhman => new LocalizedStringTerm("Misc/CharacterNames/Pezhman");
            public static LocalizedStringTerm Plumber => new LocalizedStringTerm("Misc/CharacterNames/Plumber");
            public static LocalizedStringTerm Postman => new LocalizedStringTerm("Misc/CharacterNames/Postman");
            public static LocalizedStringTerm Woman => new LocalizedStringTerm("Misc/CharacterNames/Woman");
            public static LocalizedStringTerm BabyBoy => new LocalizedStringTerm("Misc/CharacterNames/BabyBoy");
            public static LocalizedStringTerm BabyGirl => new LocalizedStringTerm("Misc/CharacterNames/BabyGirl");
            public static LocalizedStringTerm ManScifi => new LocalizedStringTerm("Misc/CharacterNames/ManScifi");
            public static LocalizedStringTerm UglyElham => new LocalizedStringTerm("Misc/CharacterNames/UglyWoman");
            public static LocalizedStringTerm Pirate => new LocalizedStringTerm("Misc/CharacterNames/Pirate");
            public static LocalizedStringTerm FemaleParrot => new LocalizedStringTerm("Misc/CharacterNames/FemaleParrot");
        }

        public static class UI_General
        {
            public static LocalizedStringTerm Later => new LocalizedStringTerm("UI/General/Later");
            public static LocalizedStringTerm No => new LocalizedStringTerm("UI/General/No");
            public static LocalizedStringTerm Ok => new LocalizedStringTerm("UI/General/Ok");
            public static LocalizedStringTerm Yes => new LocalizedStringTerm("UI/General/Yes");
            public static LocalizedStringTerm Cancel => new LocalizedStringTerm("UI/General/Cancel");
            public static LocalizedStringTerm Cancel2 => new LocalizedStringTerm("UI/General/Cancel2");
            public static LocalizedStringTerm Again => new LocalizedStringTerm("UI/General/Again");
            public static LocalizedStringTerm Oh => new LocalizedStringTerm("UI/General/Oh");
            public static LocalizedStringTerm IWant => new LocalizedStringTerm("UI/General/IWant");
            public static LocalizedStringTerm LeaveIt => new LocalizedStringTerm("UI/General/LeaveIt");
            public static LocalizedStringTerm Settings => new LocalizedStringTerm("UI/General/Settings");
            public static LocalizedStringTerm Free => new LocalizedStringTerm("UI/General/Free");
            public static LocalizedStringTerm Update => new LocalizedStringTerm("UI/General/Update");
            public static LocalizedStringTerm Download => new LocalizedStringTerm("UI/General/Download");
            public static LocalizedStringTerm Exit => new LocalizedStringTerm("UI/General/Exit");
            public static LocalizedStringTerm Nice => new LocalizedStringTerm("UI/General/Nice");
            public static LocalizedStringTerm IsFull => new LocalizedStringTerm("UI/General/IsFull");
            public static LocalizedStringTerm Booster => new LocalizedStringTerm("UI/General/Booster");
            public static LocalizedStringTerm Powerup => new LocalizedStringTerm("UI/General/Powerup");
        }

        public static class UI_NeighborhoodChallenge
        {
            public static LocalizedStringTerm NeighborhoodChallengeName => new LocalizedStringTerm("UI/NeighborhoodChallenge/NeighborhoodChallengeName");
        }

        public static class UI_LevelInfo
        {
            public static LocalizedStringTerm Hard => new LocalizedStringTerm("UI/LevelInfo/Hard");
            public static LocalizedStringTerm HardLevel => new LocalizedStringTerm("UI/LevelInfo/HardLevel");
            public static LocalizedStringTerm Level => new LocalizedStringTerm("UI/LevelInfo/Level");
            public static LocalizedStringTerm NormalLevel => new LocalizedStringTerm("UI/LevelInfo/NormalLevel");
            public static LocalizedStringTerm VeryHard => new LocalizedStringTerm("UI/LevelInfo/VeryHard");
            public static LocalizedStringTerm VeryHardLevel => new LocalizedStringTerm("UI/LevelInfo/VeryHardLevel");
            public static LocalizedStringTerm DoYouWantToWatchAdToGetBoosterOrPowerup => new LocalizedStringTerm("UI/LevelInfo/DoYouWantToWatchAdToGetBoosterOrPowerup");
            public static LocalizedStringTerm YouGotARewardForThisLevel => new LocalizedStringTerm("UI/LevelInfo/YouGotARewardForThisLevel");
        }

        public static class UI_Misc
        {
            public static LocalizedStringTerm Person => new LocalizedStringTerm("UI/Misc/Person");
            public static LocalizedStringTerm Persons => new LocalizedStringTerm("UI/Misc/Persons");
            public static LocalizedStringTerm Rewards => new LocalizedStringTerm("UI/Misc/Rewards");
            public static LocalizedStringTerm To => new LocalizedStringTerm("UI/Misc/To");
            public static LocalizedStringTerm ToTheEnd => new LocalizedStringTerm("UI/Misc/ToTheEnd");
        }

        public static class UI_Tasks
        {
            public static LocalizedStringTerm Day => new LocalizedStringTerm("UI/Tasks/Day");
        }

        public static class UI_PiggyBank
        {
            public static LocalizedStringTerm PiggyBankSafeIsFull => new LocalizedStringTerm("UI/PiggyBank/PiggyBankSafeIsFull");
            public static LocalizedStringTerm PiggyBankFirstCheckpointReached => new LocalizedStringTerm("UI/PiggyBank/PiggyBankFirstCheckpointReached");
            public static LocalizedStringTerm PiggyBankSafeIsEmpty => new LocalizedStringTerm("UI/PiggyBank/PiggyBankSafeIsEmpty");
            public static LocalizedStringTerm PiggyBankOpened => new LocalizedStringTerm("UI/PiggyBank/PiggyBankOpened");
            public static LocalizedStringTerm PiggyBankPurchased => new LocalizedStringTerm("UI/PiggyBank/PiggyBankPurchased");
            public static LocalizedStringTerm RewardFormula => new LocalizedStringTerm("UI/PiggyBank/RewardFormula");
        }

        public static class LiveOps
        {
            public static class Message_DogTraining
            {
                public static LocalizedStringTerm StagePassed => new LocalizedStringTerm("UI/DogTraining/DogTrainingStagePassed");
                public static LocalizedStringTerm EventCompleted => new LocalizedStringTerm("UI/DogTraining/DogTrainingAllStagesPassed");
            }

            public static class Message_SeasonPass
            {
                public static LocalizedStringTerm LockedPrizeBubbleMessage => new LocalizedStringTerm("UI/SeasonPass/LockedPrizeBubbleMessage");
                public static LocalizedStringTerm NotPassedPrizeBubbleMessage => new LocalizedStringTerm("UI/SeasonPass/NotPassedPrizeBubbleMessage");
                public static LocalizedStringTerm FinalPrizeBubbleMessage => new LocalizedStringTerm("UI/SeasonPass/FinalPrizeBubbleMessage");
                public static LocalizedStringTerm ClaimableBubbleMessage => new LocalizedStringTerm("UI/SeasonPass/ClaimableBubbleMessage");
            }
        }

        public static class Clan
        {
            public static class Tab
            {
                public static class AboutClan
                {
                    public static LocalizedStringTerm Title => new LocalizedStringTerm("Clan/Tab/AboutClan/Title");
                }

                public static class Leaderboard
                {
                    public static LocalizedStringTerm ServerUpdateRateText => new LocalizedStringTerm("Clan/Tab/Leaderboard/ServerIsUpdatedEachXMinutes");
                }

                public static class Chat
                {
                    public static class Prefabs
                    {
                        public static LocalizedGameObjectTerm DefaultMessageTerm => new LocalizedGameObjectTerm("Clan/Chat/Prefabs/DefaultMessage");
                        public static LocalizedGameObjectTerm MyDefaultMessageTerm => new LocalizedGameObjectTerm("Clan/Chat/Prefabs/MyDefaultMessage");
                        public static LocalizedGameObjectTerm JoinedMessageTerm => new LocalizedGameObjectTerm("Clan/Chat/Prefabs/JoinedMessage");
                        public static LocalizedGameObjectTerm LeftMessageTerm => new LocalizedGameObjectTerm("Clan/Chat/Prefabs/LeftMessage");
                        public static LocalizedGameObjectTerm KickedMessageTerm => new LocalizedGameObjectTerm("Clan/Chat/Prefabs/KickedMessage");
                        public static LocalizedGameObjectTerm LifeExchangeMessageTerm => new LocalizedGameObjectTerm("Clan/Chat/Prefabs/LifeExchangeMessage");
                        public static LocalizedGameObjectTerm MyLifeExchangeMessageTerm => new LocalizedGameObjectTerm("Clan/Chat/Prefabs/MyLifeExchangeMessage");
                        public static LocalizedGameObjectTerm JoinRequestMessageTerm => new LocalizedGameObjectTerm("Clan/Chat/Prefabs/JoinRequestMessage");

                        public static class Base
                        {
                            public static LocalizedGameObjectTerm HelpCoinTerm => new LocalizedGameObjectTerm("Clan/Chat/Prefabs/Base/HelpCoin");
                        }
                    }
                }
            }

            public static class Error
            {
                public static class CreateTeam
                {
                    public static LocalizedStringTerm EmptyTeamName => new LocalizedStringTerm("Clan/Error/CreateTeam/EmptyTeamName");
                    public static LocalizedStringTerm InvalidTeamName => new LocalizedStringTerm("Clan/Error/CreateTeam/InvalidTeamName");
                    public static LocalizedStringTerm HigherThanYourOwnLevel => new LocalizedStringTerm("Clan/Error/CreateTeam/HigherThanYourOwnLevel");
                    public static string LowerThanMinLevel => string.Format(new LocalizedStringTerm("Clan/Error/CreateTeam/LowerThanMinLevel").ToString(), ClanUnlockLevel);

                    private static int ClanUnlockLevel => ServiceLocator.Find<ServerConfigManager>().data.config.clanServerConfig.UnlockLevel;
                }

                public static class Joining
                {
                    public static LocalizedStringTerm FirstLeaveYourOwnTeam => new LocalizedStringTerm("Clan/Error/Joining/FirstLeaveYourOwnTeam");
                    public static LocalizedStringTerm ThisIsYourTeam => new LocalizedStringTerm("Clan/Error/Joining/ThisIsYourTeam");
                }

                public static class Shared
                {
                    public static LocalizedStringTerm ServerErrorEncountered => new LocalizedStringTerm("Clan/Error/Shared/ServerErrorEncountered");
                    public static LocalizedStringTerm BadRequestError => new LocalizedStringTerm("Clan/Error/Shared/BadRequestError");
                }

                public static class Inbox
                {
                    public static LocalizedStringTerm YourLivesAreAlreadyFull => new LocalizedStringTerm("Clan/Error/Inbox/YourLivesAreAlreadyFull");
                }
            }

            public static class Shared
            {
                public static class TeamType
                {
                    public static LocalizedStringTerm Private => new LocalizedStringTerm("Clan/Tab/CreateTeam/Close");
                    public static LocalizedStringTerm Public => new LocalizedStringTerm("Clan/Tab/CreateTeam/Open");
                }

                public static class Joining
                {
                    public static LocalizedStringTerm RequestSent => new LocalizedStringTerm("Clan/Shared/Joining/RequestSent");
                }
            }

            public static class Inbox
            {
                public static class Donation
                {
                    public static LocalizedStringTerm DonationMessage => new LocalizedStringTerm("Clan/Inbox/Donation/DonationMessage");
                }
            }
        }
    }
}