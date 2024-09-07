using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class EntranceConfig : BaseItemConfig
{
    class EntranceData : IItemData
    {
        public ItemType GetItemType()
        {
            return ItemType.EntranceItem;
        }
    }

    public abstract List<CellAttachment> TileSources(CellAttachmentFactory factory, BoardConfig boardConfig);


    public override IItemData GetItemData()
    {
        return new EntranceData();
    }
}


[CreateAssetMenu(menuName = "Baord/CellItems/ButterflyEntrance")]
public class ButterflyEntranceConfig : EntranceConfig
{
    public override List<CellAttachment> TileSources(CellAttachmentFactory factory, BoardConfig boardConfig)
    {
        return new List<CellAttachment> { factory.CreateButterflyTileSource() };
    }
}
