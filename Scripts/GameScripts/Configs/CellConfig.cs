using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Dir { None, Left, Right, Up, Down}

[System.Serializable]
public class CellConfig
{
    #region public fields
    public BaseItemConfig[] itemsArr = null;
    public BaseItemConfig[] cItemsArr = null;
    public bool haveBoat = false;
    public int riverExitIndex = -1;
    public int portalExitIndex = -1;
    #endregion
}