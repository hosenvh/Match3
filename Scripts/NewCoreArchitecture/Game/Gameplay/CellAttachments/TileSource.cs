using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.CellAttachments
{
    public abstract class TileSource : CellAttachment
    {
        public abstract Type TileType();

        public abstract int GenerationYOffset();
    }

    public abstract class SpecificTileSource<T> : TileSource where T : Tile
    {

        public override Type TileType()
        {
            return typeof(T);
        }
    }

    public class ColoredBeadTileSource : SpecificTileSource<ColoredBead>
    {
        public override int GenerationYOffset()
        {
            return -1;
        }
    }

    public class LemonadeTileSource : SpecificTileSource<Lemonade>
    {
        public override int GenerationYOffset()
        {
            return -1;
        }
    }

    public class NutTileSource : SpecificTileSource<Nut>
    {
        public override int GenerationYOffset()
        {
            return -1;
        }
    }

    public class JamJarTileSource : SpecificTileSource<JamJar>
    {
        public override int GenerationYOffset()
        {
            return -1;
        }
    }

    public class ButterflyTileSource : SpecificTileSource<Butterfly>
    {
        public override int GenerationYOffset()
        {
            return 1;
        }
    }

    public class RocketBoxTileSource : SpecificTileSource<RocketBox>
    {
        public override int GenerationYOffset()
        {
            return -1;
        }
    }

    public class GasCylinderTileSource : SpecificTileSource<GasCylinder>
    {
        public override int GenerationYOffset()
        {
            return -1;
        }
    }

    public class CatColoredBeadTileSource : SpecificTileSource<CatColoredBead>
    {
        public override int GenerationYOffset()
        {
            return -1;
        }
    }

    public class DynamicTileSource : TileSource
    {
        private Type type;

        public DynamicTileSource(Type type)
        {
            this.type = type;
        }

        public override Type TileType()
        {
            return type;
        }

        public override int GenerationYOffset()
        {
            return -1;
        }
    }
}
