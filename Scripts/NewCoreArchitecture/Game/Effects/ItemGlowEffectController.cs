using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Game.Effects
{
    
    
    public class ItemGlowEffectController : MonoBehaviour
    {

        public enum ObjectType
        {
            SkeletonGraphic,
            Image
        }

        [SerializeField] private ObjectType objectType = ObjectType.SkeletonGraphic;
        [SerializeField] private Image image;
        [SerializeField] private SkeletonGraphic skeletonGraphic = default;
        [SerializeField] private Material normalMaterial = default;
        [SerializeField] private Material glowMaterial = default;
        [SerializeField] private Animation glowAnimation = default;
        public float glowAmount = 1;

        private static readonly int Glow = Shader.PropertyToID("_Glow");

        
        #if UNITY_EDITOR
        void Awake()
        {
            normalMaterial = Instantiate(normalMaterial);
            glowMaterial = Instantiate(glowMaterial);
        }
        #endif
        
        [ContextMenu("Set Glow")]
        public void SetGlow()
        {
            glowAmount = 1;
            
            if (objectType == ObjectType.SkeletonGraphic)
                skeletonGraphic.material = glowMaterial;
            else
                image.material = glowMaterial;
            
            if (glowAnimation == null) return;
            glowAnimation.Play();
            StartCoroutine(Glowing());
        }

        [ContextMenu("Set Normal")]
        public void SetNormal()
        {
            if (objectType == ObjectType.SkeletonGraphic)
                skeletonGraphic.material = normalMaterial;
            else
                image.material = normalMaterial;

            if (glowAnimation == null) return;
            glowAnimation.Stop();
            StopAllCoroutines();
        }


        IEnumerator Glowing()
        {
            while (true)
            {
                glowMaterial.SetFloat(Glow, glowAmount);
                yield return null;
            }
        }
        
        
    }

}