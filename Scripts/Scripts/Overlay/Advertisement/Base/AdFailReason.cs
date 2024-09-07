namespace Match3.Overlay.Advertisement.Base
{
    public enum AdFailReason
    {
        None,
        RequestIsAlreadyInProgress,
        SessionIsAlreadyOpen,
        SessionIsNotOpen,
        InvalidZoneId,
        NoAdAvailable,
        NoConnection,
        UnknownError,
        NoAdReady,
        AdPlayerNotAvailableInBuild,
        TimeOut,
        InvalidAdvertisementPlayer
    }
}