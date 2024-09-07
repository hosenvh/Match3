using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/Hedge")]
public class HedgeConfig : BaseItemConfig
{
    public int level;
    public class Data : IItemData
    {
        public ItemType GetItemType()
        {
            return ItemType.Hedge;
        }
    }

    public override IItemData GetItemData()
    {
        return new Data();
    }
}
