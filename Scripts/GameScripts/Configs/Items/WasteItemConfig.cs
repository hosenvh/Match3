
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Waste")]
public class WasteItemConfig : BaseItemConfig
{
    public int level;
    public override IItemData GetItemData()
    {
        return new WasteItemData();
    }
}