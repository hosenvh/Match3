using System;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    // TODO: Rename this.
    public class Counter
    {
        int amount;
        Action onFinished;

        public Counter(int amount, Action onFinished)
        {
            this.amount = amount;
            this.onFinished = onFinished;
        }

        public void ForceFinish()
        {
            amount = 0;
            onFinished();
            onFinished = null;
        }

        public void SetAmount(int amount)
        {
            this.amount = amount;
        }

        public void Decrement()
        {
            amount--;
            if (amount == 0)
                onFinished();
        }
    }
}