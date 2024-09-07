using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/IvyRoot")]
public class IvyRootConfig : BaseItemConfig
{
    public class Data : IItemData
    {
        public ItemType GetItemType()
        {
            return ItemType.IvyRoot;
        }
    }

    public override IItemData GetItemData()
    {
        return new Data();
    }
}