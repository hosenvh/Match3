
using UnityEngine;
using UnityEngine.UI;

public class WoodItemData : IItemData
{
    #region private fields
    [SerializeField]
    private Sprite[] imagesArr;
    #endregion

    #region properties
    public int Level { get; private set; }
    public ItemColorType ColorType { get; private set; }
    #endregion

    #region constructors
    public WoodItemData(int level, ItemColorType itemColorType)
    {
        Level = level;
        ColorType = itemColorType;
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.WoodItem; }
    #endregion
}