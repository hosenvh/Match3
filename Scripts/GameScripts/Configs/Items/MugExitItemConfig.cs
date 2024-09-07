using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/MugExit")]
public class MugExitItemConfig : BaseItemConfig
{
    #region public methods
    public override IItemData GetItemData()
    {
        return new MugExitItemData();
    }
    #endregion
}