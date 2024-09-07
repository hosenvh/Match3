using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/CatColoredBead")]
    public class CatColoredBeadConfig : BaseItemConfig, LogicalTileCreator
    {
        public TileColor tileColor;

        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.MatchItem;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }

        public Tile CreateLogicalEntity(TileFactory tileFactory)
        {
            return tileFactory.CreateCatColoredBead(tileColor);
        }
    }
}