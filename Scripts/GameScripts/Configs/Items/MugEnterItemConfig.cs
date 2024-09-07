using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/MugEnter")]
public class MugEnterItemConfig : BaseItemConfig
{
    #region public methods
    public override IItemData GetItemData()
    {
        return new MugEnterItemData();
    }
    #endregion
}