using System;

namespace Match3.Foundation.Base.Utility
{
    public class CallBackSession
    {
        Action onSuccess;
        Action onFailure;


        bool isFinished = false;

        public CallBackSession(Action onSuccess, Action onFailure)
        {
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;
        }

        public void TryFinishAsSuccess()
        {
            if (isFinished)
                return;

            isFinished = true;
            onSuccess.Invoke();
        }

        public void TryFinishAsFailure()
        {
            if (isFinished)
                return;

            isFinished = true;
            onFailure.Invoke();
        }
    }
}