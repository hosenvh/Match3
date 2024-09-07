using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PrimaryItemType { Ground, Block, River, River_Crossable, Block_Crossable }

[CreateAssetMenu(menuName = "Baord/CellItems/PrimaryItem")]
public class PrimaryItemConfig : BaseItemConfig
{
    #region private fields
    [SerializeField]
    private PrimaryItemType primaryType = default;
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new PrimaryItemData(primaryType);
    }
    #endregion
}