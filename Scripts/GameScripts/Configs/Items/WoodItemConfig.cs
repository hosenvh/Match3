using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Wood")]
public class WoodItemConfig : BaseItemConfig
{
    [SerializeField]
    private int level = 1;
    [SerializeField]
    private ItemColorType colorType = default;

    public override IItemData GetItemData()
    {
        return new WoodItemData(level, colorType);
    }

}