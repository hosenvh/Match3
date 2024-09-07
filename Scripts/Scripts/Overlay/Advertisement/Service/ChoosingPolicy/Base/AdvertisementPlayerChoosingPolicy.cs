using Match3.Overlay.Advertisement.Players.Base;


namespace Match3.Overlay.Advertisement.Service.ChoosingPolicy.Base
{
    public abstract class AdvertisementPlayerChoosingPolicy
    {
        protected readonly AdvertisementPlayersHandler playersHandler;

        protected AdvertisementPlayerChoosingPolicy(AdvertisementPlayersHandler playersHandler)
        {
            this.playersHandler = playersHandler;
        }

        public abstract void Resort();

        public abstract bool HasAnyPlayerLeft();
        public abstract AdvertisementPlayerBase GetNextPlayerAndAdvance();
        public abstract void HandleAdvertisementPlayerUsedSuccessfully(AdvertisementPlayerBase advertisementPlayer);
    }
}