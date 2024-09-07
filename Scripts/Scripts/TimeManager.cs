using SeganX;
using System;


namespace Match3
{
    public class TimeManager : ITimeManager
    {
        public event Action OnTimeSyncWithServerStateChanged = delegate { };
        public long ServerTimeDifference { get; private set; } = 0;
        public bool IsTimeSyncedWithServer
        {
            get => isTimeSyncedWithServer;
            private set
            {
                isTimeSyncedWithServer = value;
                OnTimeSyncWithServerStateChanged.Invoke();
            }
        }

        private bool isTimeSyncedWithServer;

        public void TrySyncTimeWithServer(Action onComplete)
        {
            var serverConfigRequest = new ServerConfigRequest();
            serverConfigRequest.UpdateServerTime(onComplete: s => onComplete.Invoke());
        }

        public long GetCurrentServerUnixTime()
        {
            return Utilities.NowTimeUnix() + ServerTimeDifference;
        }

        public DateTime GetCurrentServerUTCTime()
        {
            return Utilities.UnixTimeToUTCTime(GetCurrentServerUnixTime());
        }

        public DateTime GetCurrentLocalTimeTime()
        {
            return Utilities.UnixTimeToLocalTime(GetCurrentServerUnixTime());
        }

        public void RefreshTimeSyncWithServer(long serverTime)
        {
            IsTimeSyncedWithServer = true;
            ServerTimeDifference = serverTime - Utilities.NowTimeUnix();
        }

        public void MarkTimeNotSyncedWithServer()
        {
            IsTimeSyncedWithServer = false;
        }
    }
}