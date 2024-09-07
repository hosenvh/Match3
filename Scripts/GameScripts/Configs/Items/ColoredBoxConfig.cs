using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/ColoredBox")]
    public class ColoredBoxConfig : BaseItemConfig
    {
        public class ColoredBoxData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.WoodItem;
            }
        }

        public int level = 1;
        public TileColor color;

        public override IItemData GetItemData()
        {
            return new ColoredBoxData();
        }
    }


}