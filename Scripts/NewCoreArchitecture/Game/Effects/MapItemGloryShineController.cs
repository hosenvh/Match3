using System;
using UnityEngine;


[DisallowMultipleComponent]
public class MapItemGloryShineController : MonoBehaviour
{
    private const string GloryShineEffectShader = "Unlit/UnlitTransparentWithGloryShine";
    private const string ShaderShineKey = "SHINE_EFFECT";

    private static readonly int ShineWidthProp = Shader.PropertyToID("_ShineWidth");
    private static readonly int ShineSmoothProp = Shader.PropertyToID("_ShineSmooth");
    private static readonly int ShineDirProp = Shader.PropertyToID("_ShineDir");
    private static readonly int ShinePositionProp = Shader.PropertyToID("_ShinePosition");

    [SerializeField] private AnimationCurve shineAnimationCurve;
    [SerializeField] private float shineDuration;

    private Material shineMaterial;
    private bool shine = false;
    private float startShinePos;
    private float shineMoveLength = 0;
    private float spentTime = 0;

    private Action onEffectEnd;
    private float initialDefaultShinePosition;

    
    
    public bool CanShine(MeshRenderer meshRenderer)
    {
        var gloryShader = Shader.Find(GloryShineEffectShader);
        return meshRenderer.sharedMaterial.shader.Equals(gloryShader) && !shine;
    }
    
    
    public void ApplyGloryShine(MeshRenderer meshRenderer, Action onEffectEnd)
    {
        this.onEffectEnd = onEffectEnd;

        CalculateShineStartPosAndMoveLength(meshRenderer);
        
        shineMaterial = meshRenderer.sharedMaterial;
        shineMaterial.EnableKeyword(ShaderShineKey);
        initialDefaultShinePosition = shineMaterial.GetFloat(ShinePositionProp);
        shineMaterial.SetFloat(ShinePositionProp, startShinePos);
        spentTime = 0;
        shine = true;
    }

    void Update()
    {
        if (!shine) return;
        
        spentTime += Time.deltaTime;
        
        shineMaterial.SetFloat(ShinePositionProp,
            startShinePos + shineMoveLength * shineAnimationCurve.Evaluate(spentTime / shineDuration));
        
        if (spentTime > shineDuration)
        {
            ResetShineEffect();
            shine = false;
            onEffectEnd?.Invoke();
        }

        void ResetShineEffect()
        {
            shineMaterial.DisableKeyword(ShaderShineKey);
            shineMaterial.SetFloat(ShinePositionProp, initialDefaultShinePosition);
        }
    }
    
    private void CalculateShineStartPosAndMoveLength(MeshRenderer meshRenderer)
    {
        var shineWidth = meshRenderer.sharedMaterial.GetFloat(ShineWidthProp);
        var shineSmooth = meshRenderer.sharedMaterial.GetFloat(ShineSmoothProp);
        var shineSlope = meshRenderer.sharedMaterial.GetFloat(ShineDirProp);

        var bounds = meshRenderer.bounds;
        var localScale = transform.localScale;
        var meshHalfWidth = bounds.size.x / localScale.x * 0.5f;
        var meshHalfHeight = bounds.size.y / localScale.y * 0.5f;
        
        var cc = meshHalfHeight +
                 Mathf.Tan(Mathf.Deg2Rad * shineSlope) * meshHalfWidth;
        
        startShinePos = -(cc + shineWidth + shineSmooth);
        shineMoveLength = Mathf.Abs(startShinePos) * 2;
    }


}
