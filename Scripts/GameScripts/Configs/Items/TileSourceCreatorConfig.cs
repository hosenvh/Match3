using UnityEngine;


namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/CellItems/TileSourceCreator")]
    public class TileSourceCreatorConfig : BaseItemConfig
    {
        private class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.TileSourceCreator;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }
}