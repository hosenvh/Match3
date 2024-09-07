using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/VacuumCleaner")]
    public class VacuumCleanerConfig : BaseItemConfig
    {
        public class VacuumCleanerData : IItemData
        {
            public ItemType GetItemType()
            {
                return ItemType.VacuumCleaner;
            }
        }

        public TileColor targetColor;

        public override IItemData GetItemData()
        {
            return new VacuumCleanerData();
        }
    }

}