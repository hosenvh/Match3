using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    #region fields
    public float duration;
    public string animationName;

    ParticleSystem[] particleSystems;
    Spine.Unity.SkeletonGraphic[] graphics;
    Animation[] animations;
    #endregion

    #region properties
    #endregion

    #region methods
    public void Awake()
    {
        graphics = GetComponentsInChildren<Spine.Unity.SkeletonGraphic>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        animations = GetComponentsInChildren<Animation>();
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        foreach (var item in particleSystems)
            item.Play();
        foreach (var item in graphics)
            item.AnimationState.SetAnimation(0, animationName, false);
        foreach (var item in animations)
            item.Play();

        yield return new WaitForSeconds(duration);
        StartCoroutine(PlayEffect());
    }
    #endregion
}