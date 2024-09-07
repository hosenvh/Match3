using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;

namespace Match3.Main
{
    public class MainCellAtttachmentFactory : CellAttachmentFactory
    {

        public CellAttachment CreateLemonadeSink()
        {
            return new LemonadeSink();
        }

        public CellAttachment CreateColoredBeadTileSource()
        {
            return new ColoredBeadTileSource();
        }

        public CellAttachment CreateLemonadeTileSource()
        {
            return new LemonadeTileSource();
        }


        public CellAttachment CreateNutTileSource()
        {
            return new NutTileSource();
        }


        public CellAttachment CreateButterflyTileSource()
        {
            return new ButterflyTileSource();
        }

        public CellAttachment CreateRocketBoxTileSource()
        {
            return new RocketBoxTileSource();
        }

        public CellAttachment CreateGasCylinderTileSource()
        {
            return new GasCylinderTileSource();
        }

        public CellAttachment CreateJamJarTileSource()
        {
            return new JamJarTileSource();
        }

        public CellAttachment CreateWall(GridPlacement placement)
        {
            return new Wall(placement);
        }

        public CellAttachment CreateRope(GridPlacement placement, int initialLevel)
        {
            return new Rope(placement, initialLevel);
        }

        public CellAttachment CreateLilyPad()
        {
            return new LilyPad();
        }

        public CellAttachment CreateRainbowPreventer()
        {
            return new RainbowPreventer();
        }

        public CellAttachment CreatePortalEntrance(CellStack exitCellStack)
        {
            return new PortalEntrance(exitCellStack);
        }

        public CellAttachment CreatePortalExit(CellStack entranceCellStack)
        {
            return new PortalExit(entranceCellStack);
        }

        public CellAttachment CreateLilyPadBud(int growthLevel)
        {
            return new LilyPadBud(growthLevel);
        }

        public CellAttachment CreateCatColoredBeadTileSource()
        {
            return new CatColoredBeadTileSource();
        }

        public CellAttachment CreateTileSourceCreator(IEnumerable<Type> sourceTypes)
        {
            return new TileSourceCreator(sourceTypes);
        }
    }
}