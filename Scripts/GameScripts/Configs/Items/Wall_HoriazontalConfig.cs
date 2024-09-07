using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/CellItems/Wall_Horiazontal")]
public class Wall_HoriazontalConfig : BaseItemConfig
{
    public override IItemData GetItemData()
    {
        return new Wall_HorizontalItemData();
    }
}