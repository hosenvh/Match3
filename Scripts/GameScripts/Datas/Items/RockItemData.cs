
using UnityEngine;
using UnityEngine.UI;

public class RockItemData : IItemData
{
    #region properties
    public int Level { get; private set; }
    #endregion

    #region constructors
    public RockItemData(int level)
    {
        Level = level;
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.RockItem; }
    #endregion
}