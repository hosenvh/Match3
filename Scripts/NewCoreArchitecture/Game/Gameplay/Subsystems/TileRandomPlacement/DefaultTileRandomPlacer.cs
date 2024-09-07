using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Tiles.Explosives;


namespace Match3.Game.Gameplay.SubSystems.TileRandomPlacement
{
    public interface DefaultTilePlacerPresentationHandler : TilePlacerPresentationHandler
    {
        void PlaceSequence(List<Tile> tiles, List<CellStack> targets, Action<int> onPlaced, Action  onCompleted);
        void PlaceSingle(Tile tile, CellStack target, Action onCompleted);
    }

    public class DefaultTileRandomPlacer : TileRandomPlacer<DefaultTilePlacerPresentationHandler>
    {
        public DefaultTileRandomPlacer(int priority, DefaultTilePlacerPresentationHandler presentationHandler) : base(priority, presentationHandler)
        {
        }

        protected override Tile CreateTileFor(Type type)
        {
            var factory = ServiceLocator.Find<TileFactory>();

            if (type == typeof(Bomb))
                return factory.CreateBombTile();
            else if (type == typeof(Rocket))
                return factory.CreateRocketTile();
            else if (type == typeof(Dynamite))
                return factory.CreateDynamiteTile();
            else if (type == typeof(Rainbow))
                return factory.CreateRainbowTile();

            throw new Exception($"Tile {type.Name} is not supported by {GetType()} yet");
        }
    }
}