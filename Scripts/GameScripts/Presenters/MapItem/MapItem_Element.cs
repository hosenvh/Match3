using System;
using System.Collections;
using UnityEngine;


public abstract class MapItem_Element : Base
{
    
    public GameObject touchGraphicGameObject;

    protected State_Map CurrentMapState => gameManager.mapManager.CurrentMap;
    
    public abstract void ShowElementState(int index, bool withAnimation, Action onFinish);

    protected void PlayUnityAnimation(Animation animatorPrefab, bool fadeIn, Action onFinish, int fadeIndex = 0)
    {
        Animation graphicAnimator = Instantiate(animatorPrefab, transform.parent);
        graphicAnimator.transform.position = transform.position;
        transform.SetParent(graphicAnimator.transform);
        name = "element";
        
        var animationClipName = (fadeIn ? "FadeIn" : "FadeOut") + (fadeIndex > 1 ? fadeIndex.ToString() : "");
        graphicAnimator.Play(animationClipName);

        StartCoroutine(CheckForAnimationEnd());

        IEnumerator CheckForAnimationEnd()
        {
            while (graphicAnimator.isPlaying)
                yield return null;
            DoPostAnimationActions();
        }

        void DoPostAnimationActions()
        {
            transform.SetParent(graphicAnimator.transform.parent);
            Destroy(graphicAnimator.gameObject);
            if (onFinish != null)
                onFinish();
        }
    }

    protected void SetActiveAll(bool active)
    {
        if (touchGraphicGameObject)
            touchGraphicGameObject.SetActive(active);
        gameObject.SetActive(active);
    }

    public GameObject TouchGraphicGameObject()
    {
        return touchGraphicGameObject;
    }

    #if UNITY_EDITOR
    [ContextMenu("Try Found Raycast Target")]
    private void TryGetTouchDisabler()
    {
        if (touchGraphicGameObject == null)
            touchGraphicGameObject = gameObject.GetComponentInChildren<NonDrawingGraphic>()?.gameObject;
    }
    #endif
}