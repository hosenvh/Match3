public class IvySackItemData : IItemData
{
    public int Level { get; private set; }

    public IvySackItemData(int level)
    {
        Level = level;
    }

    public ItemType GetItemType()
    {
        return ItemType.IvySack;
    }
}