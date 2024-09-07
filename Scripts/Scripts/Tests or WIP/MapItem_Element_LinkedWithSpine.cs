using System;
using Spine.Unity;
using UnityEditor;
using UnityEngine;


public class MapItem_Element_LinkedWithSpine : MapItem_Element
{
    public enum ElementObjectType
    {
        Model,
        Sprite
    }

    public const string linkedSpineMainDirectory = "Prefabs/LinkedSpineElements/";

    // ----------------------------------------------- Public Fields ----------------------------------------------- \\

    public bool showTouchGraphic; 
    
    public ElementObjectType elementObjectType;
    public string linkedSpineName = "";
    
    public bool fadeAfterAnimationEnd = true;
    public bool deleteSpineAfterAnimationEnd = true;

    public Vector3 mySpineLocalPosition;

    public MeshRenderer myMeshRenderer;
    public SpriteRenderer mySprRenderer;
    
    #if UNITY_EDITOR
    
    public RectTransform mySpineRect;
    public float scaleAmount = 0.05f;
    public Color boldColor = Color.magenta;
    public bool isBold = false;
    #endif
    
    // ----------------------------------------------- Private Fields ----------------------------------------------- \\
    
    private int currentStateIndex;
    
    // -------------------------------------------------------------------------------------------------------------- \\

#if UNITY_EDITOR

    public void GetLocalSpinePosition()
    {
        if (mySpineRect == null) return;
        mySpineLocalPosition = mySpineRect.localPosition;
    }

    public void ScaleUp()
    {
        transform.localScale += Vector3.one * scaleAmount;
    }
    
    public void ScaleDown()
    {
        transform.localScale -= Vector3.one * scaleAmount;
    }

    public void ChangeActivenessOfSpine()
    {
        if (mySpineRect == null) return;
        mySpineRect.gameObject.SetActive(!mySpineRect.gameObject.activeSelf);
    }

    public void InitMySpineObject()
    {
        if (mySpineRect == null) return;
        mySpineRect.gameObject.AddComponent<LinkedElementSpine>();
        var c = mySpineRect.GetComponent<MapItem_Element_Spine>();
        if(c!=null)
            DestroyImmediate(c);
    }
    
    public void SetSpineToCorrectPosition()
    {
        if (mySpineRect != null) 
            mySpineRect.localPosition = mySpineLocalPosition;
    }

#endif


    private void Reset()
    {
        FindComponentsAndSetType();
    }
    
    private void Start()
    {
        FindComponentsAndSetType();
    }


    public override void ShowElementState(int index, bool withAnimation, Action onFinish)
    {
        if (index == 0 && !withAnimation)
        {
            SetActiveAll(false);
            onFinish?.Invoke();
        }
        else
        {
            SetActiveAll(true);

            if (withAnimation)
            {
                myMeshRenderer.enabled = false;
                
                var spine = Instantiate((GameObject)Resources.Load(linkedSpineMainDirectory+linkedSpineName), transform.parent)
                    .GetComponent<LinkedElementSpine>();
                spine.RectTransform.localPosition = mySpineLocalPosition;
                spine.StartAnimation(index, () =>
                {
                    myMeshRenderer.enabled = true;
                    
                    if (index == 0)
                    {
                        SetActiveAll(false);
                    }
                    else if (fadeAfterAnimationEnd && ObjectToObjectFader.Instance!=null)
                    {
                        if (elementObjectType == ElementObjectType.Model)
                            ObjectToObjectFader.Instance.SpineFadeToModel(spine.GetComponent<SkeletonGraphic>(), 
                                myMeshRenderer, () =>
                                {
                                    if (deleteSpineAfterAnimationEnd)
                                        Destroy(spine.gameObject);
                                });
                        else if(elementObjectType == ElementObjectType.Sprite)
                            ObjectToObjectFader.Instance.SpineFadeToSprite(spine.GetComponent<SkeletonGraphic>(), 
                                mySprRenderer, () =>
                                {
                                    if (deleteSpineAfterAnimationEnd)
                                        Destroy(spine.gameObject);
                                });
                    }
                    else if (deleteSpineAfterAnimationEnd)
                    {
                        Destroy(spine.gameObject);
                    }
                    
                    onFinish?.Invoke();
                });
            }
            else
            {
                onFinish?.Invoke();
            }
        }
    }
    
    private void FindComponentsAndSetType()
    {
        if (myMeshRenderer != null || mySprRenderer != null) return;
        myMeshRenderer = GetComponent<MeshRenderer>();
        mySprRenderer = GetComponent<SpriteRenderer>();
        if (myMeshRenderer != null) elementObjectType = ElementObjectType.Model;
        else if (mySprRenderer != null) elementObjectType = ElementObjectType.Sprite;
    }

}