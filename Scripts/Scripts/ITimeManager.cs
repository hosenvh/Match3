using Match3.Foundation.Base.ServiceLocating;
using System;

namespace Match3
{
    public interface ITimeManager : Service
    {
        event Action OnTimeSyncWithServerStateChanged;
        bool IsTimeSyncedWithServer { get; }
        long ServerTimeDifference { get;}

        void TrySyncTimeWithServer(Action onComplete);
        long GetCurrentServerUnixTime();
        DateTime GetCurrentServerUTCTime();
        DateTime GetCurrentLocalTimeTime();
        void RefreshTimeSyncWithServer(long serverTime);
        void MarkTimeNotSyncedWithServer();
    }
}