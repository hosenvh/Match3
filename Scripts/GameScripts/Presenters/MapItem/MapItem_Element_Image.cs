using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapItem_Element_Image : MapItem_Element
{
    bool isShowing = false;
    int currentStateIndex;

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
            if (withAnimation)
            {
                if (currentStateIndex == 0)
                    PlayUnityAnimation(CurrentMapState.mapItem_imageAnimatorPrefab, true, onFinish);
            }
        }
        else
        {
            if (withAnimation)
            {
                PlayUnityAnimation(CurrentMapState.mapItem_imageAnimatorPrefab, false, () =>
                {
                    //gameObject.SetActive(false);
                    if (onFinish != null)
                        onFinish();
                });
            }
            else
                SetActiveAll(false);
        }

        currentStateIndex = index;
    }
}