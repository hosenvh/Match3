using System;
using Match3.Data.Unity.PersistentTypes;
using static Base;


namespace Match3.Game.RainBowCountReseter
{
    // NOTE: This class is temporary, we've gaven users 500 rainbows as mid day gift on task 69 by mistake, it should be 1,
    // So now we are going to take back 4 hundreds of them,
    public class RainbowCountResetter
    {
        private readonly PersistentBool isResettingWronglyGivenRainbowsChecked = new PersistentBool("Resetting_Wrongly_Given_Rainbows_Key");
        private readonly TaskManager taskManager;
        private TaskConfig wronglyGivenRainbowTask;

        public RainbowCountResetter(TaskManager taskManager)
        {
            this.taskManager = taskManager;
        }

        public void Setup(TaskConfig wronglyGivenRainbowTask)
        {
            this.wronglyGivenRainbowTask = wronglyGivenRainbowTask;
        }

        public void Start()
        {
            if (IsCheckingOnceDone())
                return;
            if (HaveUserGainedWronglyGivenRainbows())
                RemoveWronglyGivenRainbows();
            MarkCheckingIsDone();
        }

        private bool IsCheckingOnceDone()
        {
            return isResettingWronglyGivenRainbowsChecked.Get();
        }

        private bool HaveUserGainedWronglyGivenRainbows()
        {
            return IsTheTaskWithWrongRainbowRewardDone() && !IsProbablyTheTaskWithWrongRainbowRewardSkippedWithAutoSkipScenario();

            bool IsTheTaskWithWrongRainbowRewardDone()
            {
                return taskManager.IsTaskDone(wronglyGivenRainbowTask);
            }

            bool IsProbablyTheTaskWithWrongRainbowRewardSkippedWithAutoSkipScenario()
            {
                // Because if the user hasn't used skip scenario then probably she/he has a lot more rainbows than the other boosters
                return IsRainbowCountRoughlyTheSameAsOtherBoosters();

                bool IsRainbowCountRoughlyTheSameAsOtherBoosters()
                {
                    var boosterManager = gameManager.profiler.BoosterManager;
                    int bombCount = boosterManager.GetBoosterCount(0);
                    int tntRainbowCount = boosterManager.GetBoosterCount(2);
                    int notRainbowBoostersMaxCount = Math.Max(bombCount, tntRainbowCount);

                    int rainbowCount = boosterManager.GetBoosterCount(1);

                    return rainbowCount - notRainbowBoostersMaxCount < 100;
                }
            }
        }

        private void RemoveWronglyGivenRainbows()
        {
            int rainbowBoosterIndex = 1;
            var boosterManager = gameManager.profiler.BoosterManager;
            int otherBoostersMax = Math.Max(boosterManager.GetBoosterCount(0), boosterManager.GetBoosterCount(2));
            boosterManager.SetBoosterCount(rainbowBoosterIndex, otherBoostersMax);
        }

        private void MarkCheckingIsDone()
        {
            isResettingWronglyGivenRainbowsChecked.Set(true);
        }
    }
}