using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveOnTouch : MonoBehaviour
{

    private int touchCount = 0;
    private int touchCounter = 0;
    
    void Update()
    {
        #if UNITY_EDITOR

        if(Input.GetMouseButtonDown(0))
            gameObject.SetActive(false);
        
        #endif
        
        
        touchCount = Input.touchCount;
        for (touchCounter = touchCount - 1; touchCounter >= 0; --touchCounter)
        {
            if (Input.GetTouch(touchCounter).phase == TouchPhase.Began)
                gameObject.SetActive(false);
        }
    }
    
}
