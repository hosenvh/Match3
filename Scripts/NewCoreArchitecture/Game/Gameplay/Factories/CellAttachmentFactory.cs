
using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Factories
{
    public interface CellAttachmentFactory : Service
    {
        CellAttachment CreateColoredBeadTileSource();
        CellAttachment CreateLemonadeTileSource();
        CellAttachment CreateNutTileSource();
        CellAttachment CreatePortalExit(CellStack entranceCellStack);
        CellAttachment CreateButterflyTileSource();

        CellAttachment CreateLemonadeSink();
        CellAttachment CreateWall(GridPlacement placement);
        CellAttachment CreateRope(GridPlacement placement, int initialLevel);

        CellAttachment CreateLilyPad();
        CellAttachment CreateLilyPadBud(int growthLevel);
        CellAttachment CreateRainbowPreventer();

        CellAttachment CreatePortalEntrance(CellStack exitCellStack);
        CellAttachment CreateRocketBoxTileSource();
        CellAttachment CreateGasCylinderTileSource();
        CellAttachment CreateJamJarTileSource();

        CellAttachment CreateCatColoredBeadTileSource();

        CellAttachment CreateTileSourceCreator(IEnumerable<Type> sourceTypes);
    }
}