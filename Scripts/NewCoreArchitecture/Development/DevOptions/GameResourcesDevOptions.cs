using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using SeganX;
using static Base;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Resources", priority: 9)]
    public class GameResourcesDevOptions : DevelopmentOptionsDefinition
    {
        [DevOption(commandName: "Maximize Resources", color: DevOptionColor.Green)]
        public static void MaximizeResources()
        {
            RefillHearts();
            MaximizeStars();
            MaximizeCoin();
            MaximizeBoosters();
            MaximizePowerups();
            UpdateUI();

            void MaximizeStars() => SetStars(9999);
            void MaximizeCoin() => SetCoin(9999999);

            void MaximizeBoosters()
            {
                SetBoosterCount(boosterIndex: 0, count: 9999);
                SetBoosterCount(boosterIndex: 1, count: 9999);
                SetBoosterCount(boosterIndex: 2, count: 9999);
            }

            void MaximizePowerups()
            {
                SetPowerUpCount(powerupIndex: 0, count: 9999);
                SetPowerUpCount(powerupIndex: 1, count: 9999);
                SetPowerUpCount(powerupIndex: 2, count: 9999);
            }
        }

        [DevOption(commandName: "Refill Hearts")]
        public static void RefillHearts()
        {
            gameManager.profiler.FullLifeCount();
            UpdateUI();
        }

        [DevOption(commandName: "Start Infinite Life")]
        public static void ActivateInfiniteLife(int duration)
        {
            ServiceLocator.Find<ILife>().AddInfiniteLifeSecond(Utilities.NowTimeUnix(), duration);
            UpdateUI();
        }

        [DevOption(commandName: "End Infinite Life")]
        public static void EndInfiniteLife()
        {
            var lifeManager = ServiceLocator.Find<ILife>();
            var currentInfiniteLifeDuration = lifeManager.GetInInfiniteLifeRemainingTime();
            lifeManager.AddInfiniteLifeSecond(Utilities.NowTimeUnix(), -currentInfiniteLifeDuration);

            UpdateUI();
        }

        [DevOption(commandName: "Set Stars")]
        public static void SetStars(int stars)
        {
            gameManager.profiler.SetStarCount(stars);
            UpdateUI();
        }

        [DevOption(commandName: "Set Coin", color: DevOptionColor.Yellow)]
        public static void SetCoin(int coins)
        {
            gameManager.profiler.SetCoinCount(coins);
            UpdateUI();
        }

        [DevOption(commandName: "Set PowerUp Count")]
        public static void SetPowerUpCount(int powerupIndex, int count)
        {
            gameManager.profiler.SetPowerUpCount(powerupIndex, count);
            UpdateUI();
        }

        [DevOption(commandName: "Set Booster Count")]
        public static void SetBoosterCount(int boosterIndex, int count)
        {
            gameManager.profiler.BoosterManager.SetBoosterCount(boosterIndex, count);
            UpdateUI();
        }

        [DevOption(commandName: "Get Infinity Booster")]
        public static void GetInfinityBoosters(int index, int duration)
        {
            gameManager.profiler.BoosterManager.AddInfiniteBooster(index, duration);
            UpdateUI();
        }

        private static void UpdateUI()
        {
            PropagateUpdateGuiEvent();

            void PropagateUpdateGuiEvent() => ServiceLocator.Find<EventManager>().Propagate(new UpdateGUIEvent(), sender: null);
        }
    }
}