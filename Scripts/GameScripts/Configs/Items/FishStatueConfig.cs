using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/FishStatue")]
    public class FishStatueConfig : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.FishStatue;
            }
        }

        public int level;

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }

}