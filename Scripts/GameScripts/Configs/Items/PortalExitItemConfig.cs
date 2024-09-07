using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/Portal Exit")]
public class PortalExitItemConfig : BaseItemConfig
{
    #region private fields
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new PortalExitItemData();
    }
    #endregion
}