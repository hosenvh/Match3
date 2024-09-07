using System;

namespace Match3.Game.NeighborhoodChallenge.RequestHandling
{
    public abstract class BaseFailureResult : NCFailureResult
    {
        public Action confirmAction = delegate { };
        public string message;

    }
}