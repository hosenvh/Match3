using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryItemData : IItemData
{
    #region properties
    public PrimaryItemType PrimaryType { get; private set; }
    #endregion

    #region constructors
    public PrimaryItemData(PrimaryItemType primaryType)
    {
        PrimaryType = primaryType;
    }
    #endregion

    #region public methods
    public ItemType GetItemType() { return global::ItemType.PrimaryItem; }
    #endregion
}