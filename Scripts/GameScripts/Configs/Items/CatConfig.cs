using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using UnityEngine;

namespace Match3
{

    [CreateAssetMenu(menuName = "Baord/Items/Cat")]
    public class CatConfig : BaseItemConfig, LogicalTileCreator
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.Cat;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }

        public Tile CreateLogicalEntity(TileFactory tileFactory)
        {
            return tileFactory.CreateCat();
        }
    }

}