using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.SubSystems.General
{
    public struct TileStackGeneratedEvent : GameEvent
    {
        public readonly TileStack tileStack;

        public TileStackGeneratedEvent(TileStack tileStack)
        {
            this.tileStack = tileStack;
        }
    }
}