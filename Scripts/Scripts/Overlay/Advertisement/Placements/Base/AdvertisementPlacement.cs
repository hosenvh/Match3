using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.Utility;


namespace Match3.Overlay.Advertisement.Placements.Base
{
    public interface AdvertisementPlacement
    {
        bool IsAvailable();
        void UpdateInternalSateBasedOn(GameEvent gameEvent);

        // TODO: Try to make this typesafe
        void Execute(Argument argument);

        string Name();

        AdvertisementPlacementType AdvertisementType();
    }
}