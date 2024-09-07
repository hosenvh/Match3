using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/IvyBush")]
    public class IvyBushConfig : BaseItemConfig
    {
        public class IvyBushData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.IvyBush;
            }
        }


        public override IItemData GetItemData()
        {
            return new IvyBushData();
        }
    }

}