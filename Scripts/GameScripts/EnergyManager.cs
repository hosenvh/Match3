using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    #region fields
    #endregion

    #region properties
    string starCountString = "StarCount";
    int StarCount
    {
        get { return PlayerPrefs.GetInt(starCountString, 1); }
        set { PlayerPrefs.SetInt(starCountString, value); }
    }
    #endregion

    #region methods
    #endregion
}