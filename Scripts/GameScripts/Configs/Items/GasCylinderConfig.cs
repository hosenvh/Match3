using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/GasCylinder")]
    public class GasCylinderConfig : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.GasCylinder;
            }
        }

        public TileColor color;

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }
}