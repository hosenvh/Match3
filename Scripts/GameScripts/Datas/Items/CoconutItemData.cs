public class CoconutItemData : IItemData
{
    public int Level { get; private set; }
    
    public CoconutItemData(int level)
    {
        Level = level;
    }

    public ItemType GetItemType()
    {
        return ItemType.Coconut;
    }
}