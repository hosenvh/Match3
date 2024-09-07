
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/Forbidden_Rainbow")]
public class Forbidden_RainbowItemConfig : BaseItemConfig
{
    #region public methods
    public override IItemData GetItemData()
    {
        return new Forbidden_RainbowItemData();
    }
    #endregion
}