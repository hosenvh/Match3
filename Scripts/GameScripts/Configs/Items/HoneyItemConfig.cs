using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Honey")]
public class HoneyItemConfig : BaseItemConfig
{
    public override IItemData GetItemData()
    {
        return new HoneyItemData();
    }
}


