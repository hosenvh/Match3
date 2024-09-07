using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/HoneyComb")]
    public class HoneycombConfiguration : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.HoneyItem;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }

}