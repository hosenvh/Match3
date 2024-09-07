using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Mug")]
public class MugItemConfig : BaseItemConfig
{
    #region public methods
    public override IItemData GetItemData()
    {
        return new MugItemData();
    }
    #endregion
}