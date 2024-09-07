using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/HardenedHoney")]
    public class HardenedHoneyConfig : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.HardenedHoney;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }

}