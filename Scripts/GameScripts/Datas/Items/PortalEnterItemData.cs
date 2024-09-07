using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEnterItemData : IItemData
{
    #region public methods
    public ItemType GetItemType() { return global::ItemType.PortalEnterItem; }
    #endregion
}