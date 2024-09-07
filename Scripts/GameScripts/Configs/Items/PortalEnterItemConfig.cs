using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/Portal Enter")]
public class PortalEnterItemConfig : BaseItemConfig
{
    #region private fields
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new PortalEnterItemData();
    }
    #endregion
}