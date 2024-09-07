using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceItemData : IItemData
{

    public bool CanMugEnter { get; private set; }


    public EntranceItemData(bool canMugEnter)
    {
        CanMugEnter = canMugEnter;
    }


    public ItemType GetItemType() { return ItemType.EntranceItem; }

}