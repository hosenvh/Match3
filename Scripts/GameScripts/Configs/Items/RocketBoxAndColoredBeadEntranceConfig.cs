using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/RocketBoxAndColoredBeadEntrace")]
public class RocketBoxAndColoredBeadEntranceConfig : EntranceConfig
{
    public override List<CellAttachment> TileSources(CellAttachmentFactory factory, BoardConfig boardConfig)
    {
        return new List<CellAttachment> { factory.CreateRocketBoxTileSource(), factory.CreateColoredBeadTileSource() };
    }
}