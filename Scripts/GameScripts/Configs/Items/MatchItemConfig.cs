using System.Collections;
using System.Collections.Generic;
using Match3.Game.Gameplay.Tiles;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/MatchItem")]
public class MatchItemConfig : BaseItemConfig
{
    public ItemColorType itemColorType = default;
    public ColoredBead.DirtinessState dirtinessState;


    public override IItemData GetItemData()
    {
        return new MatchItemData(itemColorType);
    }
}