using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/Buoyant")]
    public class BuoyantConfig : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.HoneyItem;
            }
        }

        public TileColor color;

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }

}