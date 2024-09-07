using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Chain")]
public class ChainItemConfig : BaseItemConfig
{
    #region private fields
    [SerializeField]
    private int level = 1;
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new ChainItemData(level);
    }
    #endregion
}