using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem_State_Related_Debuger : MonoBehaviour
{

    [SerializeField] private MapItem_State_Related myRelated = default;
    [SerializeField] private int dayToActive = default;
    [SerializeField] private int stateStep = 1;
    
    private void Start()
    {
        if (Base.gameManager.taskManager.CurrentDay < dayToActive || myRelated.HaveAnyActive) return;
        
        if (myRelated.SelectedStateIndex==-1)
            myRelated.Init();
        myRelated.SetRelatedStateStep(stateStep);
    }
}
