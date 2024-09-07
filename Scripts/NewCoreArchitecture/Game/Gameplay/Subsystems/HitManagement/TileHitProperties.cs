
using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.HitManagement
{
    public interface TileHitProperties : Component
    {
        bool AcceptsDirectHit();
        bool AcceptsSideHit();
        bool SuppressesHitToSideHit();
        bool PropagatesHitToCell();
    }
}