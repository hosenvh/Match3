
using UnityEngine;
using UnityEngine.UI;

public class MugExitItemData : IItemData
{
    #region constructors
    public MugExitItemData()
    {
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.MugExitItem; }
    #endregion
}