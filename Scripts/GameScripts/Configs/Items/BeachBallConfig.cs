using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/BeachBall")]
    public class BeachBallConfig : BaseItemConfig
    {
        public List<TileColor> colors;
        public class ItemData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.BeachBall;
            }
        }

        public override IItemData GetItemData()
        {
            return new ItemData();
        }

        public HashSet<TileColor> Colors()
        {
            return new HashSet<TileColor>(colors);
        }
    }
}