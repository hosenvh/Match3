using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseItemConfig : ScriptableObject
{
    public abstract IItemData GetItemData();
}

// NOTE: This must be integrated to BaseItemConfig gradually.
public interface LogicalTileCreator
{
    Tile CreateLogicalEntity(TileFactory tileFactory);
}