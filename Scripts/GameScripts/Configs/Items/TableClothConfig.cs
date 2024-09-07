using Match3.Game.Gameplay.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/TableCloth")]
    public class TableClothConfig : BaseItemConfig
    {
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.TableCloth;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }
    }

}