using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Ice")]
public class IceItemConfig : BaseItemConfig
{
    #region private fields
    [SerializeField]
    private int level = 1;
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new IceItemData(level);
    }
    #endregion
}
