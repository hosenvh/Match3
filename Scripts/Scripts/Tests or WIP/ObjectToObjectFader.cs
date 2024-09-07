using Spine.Unity;
using System;
using UnityEditor;
using UnityEngine;


public class ObjectToObjectFader : MonoBehaviour
{

    private static ObjectToObjectFader _instance;
    public static ObjectToObjectFader Instance 
        => _instance != null ? _instance : _instance = FindObjectOfType<ObjectToObjectFader>();

    // ------------------------------------- Public Fields ------------------------------------- \\
    
    public bool usingAnimationCurve = true;
    public AnimationCurve fadingCurve;
    
    [Space(10)] public float fadingDuration = 0.5f;

    // ------------------------------------- Private Fields ------------------------------------- \\  

    private SkeletonGraphic spineGraphic;
    private SpriteRenderer  sprRenderer;
    
    private Action onFadingEnd;
    
    private Material objMat;

    private float alpha;
    private bool spineToModelFading = false;
    private bool spineToSpriteFading = false;
    private bool fading = false;
    private float elapsedTime;
    
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");

    // ------------------------------------------------------------------------------------------ \\
    
    #region MonoBehaviour

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (!fading) return;

        if (usingAnimationCurve)
        {
            elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, fadingDuration);
            if (elapsedTime >= fadingDuration) fading = false;
            alpha = fadingCurve.Evaluate(elapsedTime / fadingDuration);
        }
        else
        {
            alpha = Mathf.Max(alpha-Time.deltaTime / fadingDuration, 0) ;
            if (alpha <= 0) fading = false;
        }
        
        spineGraphic.color = new Color(1,1,1, alpha); 
        
        if(spineToModelFading)
            objMat.SetFloat(Alpha, 1-alpha);
        else if(spineToSpriteFading)
            sprRenderer.color = new Color(1,1,1, 1-alpha);
        
        if(!fading) onFadingEnd?.Invoke();
    }

    #endregion


    private void ResetFader(Action onFadingEnd)
    {
        this.onFadingEnd = onFadingEnd;
        alpha = 1;
        elapsedTime = 0;
        spineToModelFading = false;
        spineToSpriteFading = false;
    }
    
    public void SpineFadeToModel(SkeletonGraphic spineGraphic, MeshRenderer meshRenderer, Action onFadingEnd)
    {
        if(fading || spineGraphic==null || meshRenderer==null) return;
        ResetFader(onFadingEnd);
        
        this.spineGraphic = spineGraphic;
        objMat = meshRenderer.material;

        spineToModelFading = true;
        fading = true;
    }

    public void SpineFadeToSprite(SkeletonGraphic spineGraphic, SpriteRenderer sprRenderer, Action onFadingEnd)
    {
        if(fading || spineGraphic==null || sprRenderer==null) return;
        ResetFader(onFadingEnd);
        
        this.sprRenderer = sprRenderer;
        this.spineGraphic = spineGraphic;

        spineToSpriteFading = true;
        fading = true;
    }
    
    
}
