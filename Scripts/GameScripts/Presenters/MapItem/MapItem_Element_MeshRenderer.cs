using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapItem_Element_MeshRenderer : MapItem_Element
{
    bool isShowing = false;
    int currentStateIndex;

    private void Awake()
    {
        if (!isShowing)
            gameObject.SetActive(false);
    }

    public override void ShowElementState(int index, bool withAnimation, Action onFinish)
    {
        if (index > 0)
        {
            isShowing = true;
            gameObject.SetActive(true);
            if (withAnimation)
            {
                PlayUnityAnimation(CurrentMapState.mapItem_meshRendererAnimatorPrefab, true, ()=>
                {
                    TryApplyGloryShineEffect(onFinish);
                }, index);
            }
            else
                onFinish?.Invoke();
        }
        else
        {
            if (withAnimation)
            {
                PlayUnityAnimation(CurrentMapState.mapItem_meshRendererAnimatorPrefab, false, () =>
                {
                    gameObject.SetActive(false);
                    onFinish?.Invoke();
                }, currentStateIndex);
            }
            else
                gameObject.SetActive(false);
        }

        currentStateIndex = index;
    }

    private void TryApplyGloryShineEffect(Action onFinish)
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        var gloryShineController = CurrentMapState.mapItemGloryShineController;
        if (gloryShineController.CanShine(meshRenderer))
            gloryShineController.ApplyGloryShine(meshRenderer, onFinish);
        else
            onFinish?.Invoke();
    }
    
}