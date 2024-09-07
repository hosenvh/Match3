using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapItem_Element_GameObject : MapItem_Element
{
    bool isShowing = false;

    private void Awake()
    {
        if (!isShowing)
            SetActiveAll(false);
    }

    public override void ShowElementState(int index, bool withAnimation, Action onFinish)
    {
        if (index > 0)
        {
            isShowing = true;
            SetActiveAll(true);
        }
        else
            SetActiveAll(false);

        if (onFinish != null)
            onFinish();
    }
}