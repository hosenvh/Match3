using System;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public abstract class RetriableFailureResult : BaseFailureResult
    {
        public Action retryAction;
    }
}