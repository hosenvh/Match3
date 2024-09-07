using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalExitItemData : IItemData
{
    #region public methods
    public ItemType GetItemType() { return global::ItemType.PortalExitItem; }
    #endregion
}