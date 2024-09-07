using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassItemData : IItemData
{
    #region properties
    public int Level { get; private set; }
    #endregion

    #region constructors
    public GrassItemData(int level)
    {
        Level = level;
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.GrassItem; }
    #endregion

}