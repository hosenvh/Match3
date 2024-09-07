using System.Collections;
using System.Collections.Generic;
using Match3.Game.Gameplay.Tiles;
using UnityEngine;

public class MatchItemData : IItemData
{
    public ItemColorType ColorType { get; private set; }

    public MatchItemData(ItemColorType colorType)
    {
        ColorType = colorType;
    }

    public ItemType GetItemType() { return global::ItemType.MatchItem; }
}