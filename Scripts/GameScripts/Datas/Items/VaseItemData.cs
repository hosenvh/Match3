using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaseItemData : IItemData
{
    #region properties
    #endregion

    #region constructors
    public VaseItemData()
    {
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return ItemType.VaseItem; }
    #endregion
}