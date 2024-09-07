using System;
using Match3.Foundation.Unity;
using UnityEditor;
using UnityEngine;



namespace Match3.Presentation.TransitionEffects
{
    
    [Serializable]
    public class TransitionEffectControllerResourceAsset : ResourceAsset<BaseTransitionEffectController>
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TransitionEffectControllerResourceAsset))]
    public class TransitionEffectControllerResourceAssetEditor : ResourceAssetDrawer<BaseTransitionEffectController> { }
#endif
    
    public abstract class BaseTransitionEffectController : MonoBehaviour
    {
        public abstract void StartTransition(Action onFadeInFinished, Action onFadeOutFinished);
        protected abstract void FadeIn();
        protected abstract void FadeOut();
    }
    
}


