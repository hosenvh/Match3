using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/GrassSack")]
    public class GrassSackConfig : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.GrassSack;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }
}