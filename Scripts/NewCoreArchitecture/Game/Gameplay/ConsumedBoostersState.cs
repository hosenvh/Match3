namespace Match3.Game.Gameplay
{
    public class ConsumedBoostersState
    {
        public bool hasConsumedDoubleBombBooster = false;
        public bool hasConsumedRainbowBooster = false;
        public bool hasConsumedTntRainbowBooster = false;

        public void ResetConsumableBoostersStateToNotChosen()
        {
            hasConsumedDoubleBombBooster = false;
            hasConsumedRainbowBooster = false;
            hasConsumedTntRainbowBooster = false;
        }

        public void SetConsumableBoostersStateToChosen(int boosterIndex)
        {
            if (boosterIndex == 0)
                hasConsumedDoubleBombBooster = true;
            else if (boosterIndex == 1)
                hasConsumedRainbowBooster = true;
            else if (boosterIndex == 2)
                hasConsumedTntRainbowBooster = true;
        }
    }
}