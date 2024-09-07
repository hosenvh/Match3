using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/HoneyJar")]
    public class HoneyJarConfig : BaseItemConfig
    {
        public class HoneyJarData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.HoneyJar;
            }
        }

        public override IItemData GetItemData()
        {
            return new HoneyJarData();
        }
    }


}