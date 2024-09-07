using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.SubSystems.General
{
    public struct TileGeneratedEvent : GameEvent
    {
        public readonly TileStack tileStack;
        public readonly Tile tile;

        public TileGeneratedEvent(TileStack tileStack, Tile tile)
        {
            this.tileStack = tileStack;
            this.tile = tile;
        }
    }
}