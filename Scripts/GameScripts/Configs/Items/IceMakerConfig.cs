using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/IceMaker")]
    public class IceMakerConfig : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.IceMaker;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }
}
