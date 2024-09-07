using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/CellItems/LilyPadBud")]
    public class LilyPadBudConfig : BaseItemConfig
    {
        public int initalGrowthLevel;

        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.LilyPadBud;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }

}