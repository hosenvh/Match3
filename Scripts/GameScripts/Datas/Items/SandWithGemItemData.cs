public class SandWithGemItemData : IItemData
{
    #region properties
    public int Level { get; private set; }
    #endregion

    #region constructors
    public SandWithGemItemData(int level)
    {
        Level = level;
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.SandWithGemItem; }
    #endregion
}