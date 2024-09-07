public class LouieItemData : IItemData
{
    public int Level { get; private set; }
    
    public LouieItemData(int level)
    {
        Level = level;
    }

    public ItemType GetItemType()
    {
        return ItemType.Louie;
    }
}