using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Rock")]
public class RockItemConfig : BaseItemConfig
{
    #region private fields
    [SerializeField]
    private int level = 1;
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new RockItemData(level);
    }
    #endregion
}