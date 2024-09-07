using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/VaseItem")]
public class VaseItemConfig : BaseItemConfig
{
    #region private fields
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new VaseItemData();
    }
    #endregion
}