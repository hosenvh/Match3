using System;

namespace Match3.Presentation.ShopManagement
{
    public partial class ShopPopupCreator
    {
        public class Session
        {
            Action onSuccess;
            Action<FailureResult> onFailure;

            bool isFinished = false;

            public Session(Action onSuccess, Action<FailureResult> onFailure)
            {
                this.onSuccess = onSuccess;
                this.onFailure = onFailure;
            }

            public void TryFinishWithSuccess()
            {
                if (isFinished)
                    return;
                isFinished = true;
                onSuccess.Invoke();
                Reset();
            }

            public void TryFinishWithFailure(FailureResult result)
            {
                if (isFinished)
                    return;
                isFinished = true;
                onFailure.Invoke(result);
                Reset();
            }

            private void Reset()
            {
                onSuccess = null;
                onFailure = null;
            }
        }

    }
}