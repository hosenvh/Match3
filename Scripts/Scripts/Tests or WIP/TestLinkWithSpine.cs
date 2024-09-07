using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLinkWithSpine : MonoBehaviour
{

    public MapItem_UserSelect_Single userSelect;
    
    public MapItem_Element_LinkedWithSpine linked;
    public int index = 0;
    public bool withAnim = true;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            linked.ShowElementState(index, withAnim, null);
        
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
            userSelect.ShowElement(0, true, () => { Debug.Log("aaaaaaaaaa");});
        if(Input.GetKeyDown(KeyCode.Alpha2))
            userSelect.ShowElement(1, true, null);
        if(Input.GetKeyDown(KeyCode.Alpha3))
            userSelect.ShowElement(2, true, null);
    }
}
