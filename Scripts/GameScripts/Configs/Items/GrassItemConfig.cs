using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/Grass")]
public class GrassItemConfig : BaseItemConfig
{
    [SerializeField]
    private int level = 1;


    public override IItemData GetItemData()
    {
        return new GrassItemData(level);
    }
}
