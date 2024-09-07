using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Rainbow")]
public class RainbowItemConfig : BaseItemConfig
{
    #region public methods
    public override IItemData GetItemData()
    {
        return new RainbowItemData();
    }
    #endregion
}