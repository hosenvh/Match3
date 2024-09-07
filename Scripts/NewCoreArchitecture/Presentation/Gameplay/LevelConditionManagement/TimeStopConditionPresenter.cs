using System;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using SeganX;

namespace Match3.Presentation.Gameplay.LevelConditionManagement
{
    public class TimeStopConditionPresenter : StopConditionPresenter
    {
        public LocalText remainingTimeText;

        TimeStopCondition timeStopCondition;

        public override void Setup(StopConditinon stopCondition)
        {
            timeStopCondition = stopCondition.As<TimeStopCondition>();
        }

        private void Update()
        {
            remainingTimeText.SetText(Math.Ceiling(timeStopCondition.RemainingTime()).ToString());
        }
    }
}