public class SandItemData : IItemData
{
    #region properties
    public int Level { get; private set; }
    #endregion

    #region constructors
    public SandItemData(int level)
    {
        Level = level;
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.SandItem; }
    #endregion
}
