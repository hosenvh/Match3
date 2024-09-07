
using UnityEngine;
using UnityEngine.UI;

public class ExtraMoveItemData : IItemData
{
    #region properties
    public int MoveCount { get; private set; }
    #endregion

    #region public methods
    public ExtraMoveItemData(int moveCount)
    {
        MoveCount = moveCount;
    }

    public ItemType GetItemType() { return ItemType.ExtraMoveItem; }
    #endregion
}