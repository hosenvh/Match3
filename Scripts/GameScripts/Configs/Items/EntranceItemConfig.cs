using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/Entrance")]
public class EntranceItemConfig : BaseItemConfig
{

    public bool canMugEnter = false;


    public override IItemData GetItemData()
    {
        return new EntranceItemData(canMugEnter);
    }
}
