using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_VerticalItemData : IItemData
{
    #region public methods
    public ItemType GetItemType() { return global::ItemType.Wall_VerticalItem; }
    #endregion
}