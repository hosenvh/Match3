using Match3.Game.Gameplay.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/Rope")]
public class RopeConfig : BaseItemConfig
{
    public class ItemData : IItemData
    {
        private GridPlacement placement;

        public ItemData(GridPlacement placement)
        {
            this.placement = placement;
        }

        public ItemType GetItemType()
        {
            switch(placement)
            {
                case GridPlacement.Down:
                    return ItemType.Rope_Horizontal;
                case GridPlacement.Left:
                    return ItemType.Rope_Vertical;
            }

            throw new System.Exception($"Direction {placement} is not supported");
        }
    }

    public GridPlacement placement;
    public int level;

    public override IItemData GetItemData()
    {
        return new ItemData(placement);
    }

}