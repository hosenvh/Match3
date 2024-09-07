
using UnityEngine;
using UnityEngine.UI;

public class IceItemData : IItemData
{
    #region properties
    public int Level { get; private set; }
    #endregion

    #region public methods
    public IceItemData(int level)
    {
        Level = level;
    }

    public ItemType GetItemType() { return ItemType.IceItem; }
    #endregion
}
