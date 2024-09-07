using Match3.Game.Gameplay.Tiles;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/Padlock")]
    public class PadlockConfig : BaseItemConfig
    {
        public Padlock.Status status;

        public class PadlockData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.Padlock;
            }
        }

        public override IItemData GetItemData()
        {
            return new PadlockData();
        }
    }

}