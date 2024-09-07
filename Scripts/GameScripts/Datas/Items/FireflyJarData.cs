public class FireflyJarData : IItemData
{

    public int FireflyCount { get; private set; }

    public FireflyJarData(int count)
    {
        FireflyCount = count;
    }

    public ItemType GetItemType() { return ItemType.None; }
}