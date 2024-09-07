using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/CatPathCell")]
public class CatPathCellConfig : BaseItemConfig
{
    public class Data : IItemData
    {
        public ItemType GetItemType()
        {
            return ItemType.CatPathCell;
        }
    }

    public override IItemData GetItemData()
    {
        return new Data();
    }
}