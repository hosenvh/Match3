using System;
using Match3.CloudSave;
using I2.Loc;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Match3/Cloud Save/Status Description")]
public class CloudSaveStatusDescription : ScriptableObject
{
    public AuthenticationStatus forAuthStatus;
    public LocalizedStringTerm statusLocalizedDescription;


    public bool canSave;
    public bool canLoad;
}