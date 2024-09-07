using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/Wall_Vertical")]
public class Wall_VerticalConfig : BaseItemConfig
{
    #region public methods
    public override IItemData GetItemData()
    {
        return new Wall_VerticalItemData();
    }
    #endregion
}
