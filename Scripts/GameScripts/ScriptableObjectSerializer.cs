using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3;
using Match3.Utility.GolmoradLogging;


public class ScriptableObjectSerializer : MonoBehaviour
{
    public ScriptableObject objectToSerialize;

    [ContextMenu("Serialize")]
    public void Serialize()
    {
        DebugPro.LogInfo<ServerLogTag>(JsonUtility.ToJson(objectToSerialize));
    }
}
