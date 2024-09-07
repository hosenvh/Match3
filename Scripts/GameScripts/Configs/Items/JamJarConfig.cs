using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/JamJar")]
    public class JamJarConfig : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.JamJar;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }

}