using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/Duck")]
    public class DuckConfig : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType ()
            {
                return ItemType.Duck;
            }
        }

        public override IItemData GetItemData ()
        {
            return new ItemData();
        }
    }
}