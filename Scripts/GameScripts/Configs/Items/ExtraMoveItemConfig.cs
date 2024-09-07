using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/ExtraMove")]
public class ExtraMoveItemConfig : BaseItemConfig
{
    #region private fields
    [SerializeField]
    private int moveCount = 1;
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new ExtraMoveItemData(moveCount);
    }
    #endregion
}