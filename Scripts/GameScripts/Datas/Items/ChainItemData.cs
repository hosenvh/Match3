
using UnityEngine;
using UnityEngine.UI;

public class ChainItemData : IItemData
{
    #region private fields
    [SerializeField]
    private Sprite[] imagesArr;
    #endregion

    #region properties
    public int Level { get; private set; }
    #endregion

    #region constructors
    public ChainItemData(int level)
    {
        Level = level;
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return ItemType.ChainItem; }
    #endregion
}