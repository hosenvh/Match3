
public class ExplosiveItemData : IItemData
{
    #region public fields
    #endregion

    #region private fields
    #endregion

    #region properties
    public ExplosiveItemType ExplosiveType { get; private set; }
    public ItemColorType ColorType { get; private set; }
    #endregion

    #region constructors
    public ExplosiveItemData(ExplosiveItemType explosiveType, ItemColorType colorType)
    {
        ExplosiveType = explosiveType;
        ColorType = colorType;
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return ItemType.ExplosiveItem; }
    #endregion
}